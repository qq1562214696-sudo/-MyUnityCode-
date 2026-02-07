bl_info = {
    "name": "ZW_Blender: 批量FBX转换器 (优化版)",
    "author": "ZW",
    "version": (2, 2, 1),
    "blender": (3, 0, 0),
    "location": "3D视图 > 右侧面板 > ZW_Blender",
    "description": "批量转换模型文件为FBX格式 - 优化输出与日志显示",
    "category": "Import-Export",
}

import bpy
import os
import traceback
import shutil
import tempfile
import subprocess
import time
from pathlib import Path
from bpy.props import StringProperty, CollectionProperty, BoolProperty, IntProperty
from bpy.types import Operator, Panel, PropertyGroup
import atexit

# ============================================================================
# 1. 修复的日志管理器 (使用安全的单例模式)
# ============================================================================
class ZW_ConversionLog:
    _instance = None
    
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._initialized = False
        return cls._instance
    
    def __init__(self):
        # 防止重新初始化
        if getattr(self, '_initialized', False):
            return
            
        self.logs = []
        self.max_logs = 1000
        self.callback = None
        self.temp_file = None  # 临时文件路径
        self._initialized = True
    
    def add(self, level, message, filepath="", details=""):
        """添加日志条目"""
        # 确保logs属性存在 (防御性编程)
        if not hasattr(self, 'logs'):
            self.logs = []
        
        log_entry = {
            'level': level,
            'message': str(message),
            'filepath': str(filepath),
            'details': str(details),
            'time': self._get_timestamp()
        }
        
        self.logs.append(log_entry)
        
        # 限制日志数量
        if len(self.logs) > self.max_logs:
            self.logs = self.logs[-self.max_logs:]
        
        self._print_log(log_entry)
        
        # 回调UI更新
        if self.callback:
            try:
                self.callback()
            except:
                pass
    
    def clear(self):
        """清空所有日志"""
        if hasattr(self, 'logs'):
            self.logs = []
        if self.callback:
            try:
                self.callback()
            except:
                pass
    
    def get_recent(self, count=10):
        """获取最近的日志条目 - 修复方法缺失问题"""
        if not hasattr(self, 'logs'):
            self.logs = []
        if not self.logs:
            return []
        return self.logs[-count:] if count > 0 else []
    
    def get_errors(self):
        """获取所有错误日志"""
        if not hasattr(self, 'logs'):
            self.logs = []
        return [log for log in self.logs if log['level'] == 'ERROR']
    
    def get_summary(self):
        """获取统计摘要"""
        if not hasattr(self, 'logs'):
            self.logs = []
        
        total = len(self.logs)
        success = len([log for log in self.logs if log['level'] == 'SUCCESS'])
        errors = len([log for log in self.logs if log['level'] == 'ERROR'])
        warnings = len([log for log in self.logs if log['level'] == 'WARNING'])
        
        return {
            'total': total,
            'success': success,
            'errors': errors,
            'warnings': warnings
        }
    
    def get_formatted_summary(self):
        """获取格式化摘要 - 修复中文乱码问题"""
        if not hasattr(self, 'logs'):
            self.logs = []
        
        success_files = []
        failed_files = []
        failed_details = []
        
        for log in self.logs:
            if log['level'] == 'SUCCESS' and log['filepath']:
                filename = os.path.basename(log['filepath'])
                if filename not in success_files:
                    success_files.append(filename)
            elif log['level'] == 'ERROR' and log['filepath']:
                filename = os.path.basename(log['filepath'])
                if filename not in failed_files:
                    failed_files.append(filename)
                # 构建详细错误信息
                detail = f"{filename}: {log['message']}"
                if log['details']:
                    # 取第一行错误详情
                    detail_lines = log['details'].strip().split('\n')
                    if detail_lines:
                        detail += f" | 详情: {detail_lines[0][:150]}"
                failed_details.append(detail)
        
        # 修复中文乱码：确保使用UTF-8编码构建字符串
        summary = "批量FBX转换结果\n"
        summary += "=" * 50 + "\n\n"
        
        # 成功文件列表
        summary += "成功:\n"
        if success_files:
            for f in success_files:
                summary += f"{f}\n"
        else:
            summary += "无\n"
        
        summary += "\n失败:\n"
        if failed_files:
            for f in failed_files:
                summary += f"{f}\n"
        else:
            summary += "无\n"
        
        summary += "\n失败详情:\n"
        if failed_details:
            for i, detail in enumerate(failed_details, 1):
                summary += f"{i}. {detail}\n"
        else:
            summary += "无\n"
        
        summary += "\n" + "=" * 50 + "\n"
        
        # 添加统计信息
        stats = self.get_summary()
        summary += f"总计: {stats['total']}, 成功: {stats['success']}, 失败: {stats['errors']}\n"
        
        if self.logs:
            summary += f"开始时间: {self.logs[0]['time'] if self.logs else ''}\n"
            summary += f"结束时间: {self.logs[-1]['time'] if self.logs else ''}\n"
        
        return summary
    
    def save_to_temp_file_and_open(self, output_dir):
        """将日志保存到临时文件并打开，然后删除"""
        try:
            # 获取日志内容
            log_content = self.get_formatted_summary()
            
            # 创建临时文件
            timestamp = time.strftime("%Y%m%d_%H%M%S")
            temp_filename = f"转换日志_{timestamp}.txt"
            temp_filepath = os.path.join(output_dir, temp_filename)
            
            # 保存到文件
            with open(temp_filepath, 'w', encoding='utf-8') as f:
                f.write(log_content)
            
            # 记录文件路径以便后续删除
            self.temp_file = temp_filepath
            
            # 打开文件
            self._open_file(temp_filepath)
            
            # 延迟删除文件（3秒后）
            bpy.app.timers.register(lambda: self._delete_temp_file(), first_interval=3.0)
            
            return True, f"日志已保存到: {temp_filename}"
            
        except Exception as e:
            return False, f"保存日志失败: {str(e)}"
    
    def _open_file(self, filepath):
        """打开文件"""
        try:
            if os.name == 'nt':  # Windows
                os.startfile(filepath)
            elif os.name == 'posix':  # macOS/Linux
                if shutil.which('open'):  # macOS
                    subprocess.call(['open', filepath])
                elif shutil.which('xdg-open'):  # Linux
                    subprocess.call(['xdg-open', filepath])
        except Exception as e:
            log_manager.add('WARNING', f"无法自动打开文件: {str(e)}")
    
    def _delete_temp_file(self):
        """删除临时文件"""
        try:
            if self.temp_file and os.path.exists(self.temp_file):
                os.remove(self.temp_file)
                self.temp_file = None
        except Exception as e:
            log_manager.add('WARNING', f"删除临时文件失败: {str(e)}")
    
    def _get_timestamp(self):
        """获取时间戳"""
        return time.strftime("%H:%M:%S")
    
    def _print_log(self, log_entry):
        """打印日志到控制台"""
        prefix = f"[{log_entry['time']}] [{log_entry['level']}]"
        if log_entry['filepath']:
            filename = os.path.basename(log_entry['filepath'])
            print(f"{prefix} {filename}: {log_entry['message']}")
            if log_entry['details'] and log_entry['level'] in ['ERROR', 'WARNING']:
                print(f"    详情: {log_entry['details'][:200]}...")
        else:
            print(f"{prefix} {log_entry['message']}")

# 全局日志实例 - 修复单例问题
log_manager = ZW_ConversionLog()

# ============================================================================
# 2. 属性组
# ============================================================================
class ZW_ConversionResult(PropertyGroup):
    filepath: StringProperty(name="原始文件")
    success: BoolProperty(name="成功", default=False)
    message: StringProperty(name="消息", default="")
    output_path: StringProperty(name="输出路径", default="")

# ============================================================================
# 3. 文件处理器 (基于Blender官方推荐方案)
# ============================================================================
class ZW_FileProcessor:
    
    @staticmethod
    def is_supported_format(filename):
        """检查是否为支持的格式 (参考Blender官方支持列表)"""
        supported_extensions = {
            '.obj', '.fbx', '.blend', '.gltf', '.glb', 
            '.dae', '.3ds', '.ply', '.stl', '.abc',
            '.usd', '.usda', '.usdc', '.usdz', '.x3d', '.wrl'
        }
        ext = os.path.splitext(filename)[1].lower()
        return ext in supported_extensions
    
    @staticmethod
    def import_file(filepath):
        """导入文件 - 使用官方推荐方法"""
        ext = os.path.splitext(filepath)[1].lower()
        
        try:
            # 确保在对象模式
            if bpy.context.mode != 'OBJECT':
                bpy.ops.object.mode_set(mode='OBJECT')
            
            # 清空选择
            bpy.ops.object.select_all(action='DESELECT')
            
            # 记录导入前的对象数量
            objects_before = set(bpy.data.objects)
            
            # 根据格式调用对应的导入器
            if ext == '.obj':
                # OBJ格式 - 使用推荐设置
                bpy.ops.wm.obj_import(
                    filepath=filepath,
                    forward_axis='NEGATIVE_Z',
                    up_axis='Y'
                )
            elif ext == '.fbx':
                # FBX格式 - 适合动画和骨骼
                bpy.ops.import_scene.fbx(filepath=filepath)
            elif ext == '.blend':
                # Blend文件使用追加方式
                with bpy.data.libraries.load(filepath, link=False) as (data_from, data_to):
                    data_to.objects = data_from.objects
                # 链接到当前场景
                for obj in data_to.objects:
                    if obj:
                        bpy.context.collection.objects.link(obj)
            elif ext in ['.gltf', '.glb']:
                # glTF格式 - 适合PBR材质
                bpy.ops.import_scene.gltf(filepath=filepath)
            elif ext == '.dae':
                bpy.ops.wm.collada_import(filepath=filepath)
            elif ext == '.3ds':
                bpy.ops.import_scene.autodesk_3ds(filepath=filepath)
            elif ext == '.ply':
                bpy.ops.import_mesh.ply(filepath=filepath)
            elif ext == '.stl':
                # STL格式 - 适合CAD和3D打印
                bpy.ops.import_mesh.stl(filepath=filepath)
            elif ext in ['.usd', '.usda', '.usdc', '.usdz']:
                bpy.ops.wm.usd_import(filepath=filepath)
            elif ext == '.abc':
                # Alembic格式 - 适合复杂场景数据
                bpy.ops.wm.alembic_import(filepath=filepath)
            else:
                return False, f"不支持的格式: {ext}"
            
            # 检查导入的对象
            objects_after = set(bpy.data.objects)
            imported_objects = objects_after - objects_before
            
            if imported_objects:
                return True, f"导入成功: {len(imported_objects)}个对象"
            else:
                return False, "没有对象被导入"
                
        except Exception as e:
            error_details = traceback.format_exc()
            log_manager.add('ERROR', f"导入失败: {str(e)}", filepath, error_details)
            return False, f"导入异常: {str(e)}"
    
    @staticmethod
    def export_to_fbx(output_path, use_selection=True):
        """导出为FBX - 使用稳定设置"""
        try:
            # 确保输出目录存在
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            
            # 如果没有选中对象，选择所有
            if not bpy.context.selected_objects and not use_selection:
                bpy.ops.object.select_all(action='SELECT')
            
            # 备份当前选择和活动对象
            original_selection = list(bpy.context.selected_objects)
            original_active = bpy.context.view_layer.objects.active
            
            # FBX导出设置 - 使用官方推荐参数
            # FBX格式最适合导出带有骨骼和动画的对象到其他3D软件
            export_settings = {
                'filepath': output_path,
                'use_selection': use_selection,
                'object_types': {'MESH', 'ARMATURE', 'EMPTY', 'OTHER'},
                'use_mesh_modifiers': True,
                'mesh_smooth_type': 'FACE',
                'use_mesh_edges': False,
                'use_tspace': False,
                'use_custom_props': False,
                'add_leaf_bones': False,
                'primary_bone_axis': 'Y',
                'secondary_bone_axis': 'X',
                'use_armature_deform_only': False,
                'armature_nodetype': 'NULL',
                'bake_anim_use_all_bones': True,
                'bake_anim_use_nla_strips': True,
                'bake_anim_use_all_actions': True,
                'bake_anim_step': 1.0,
                'bake_anim_simplify_factor': 1.0,
                'path_mode': 'AUTO',
                'embed_textures': False,
                'batch_mode': 'OFF',
                'use_batch_own_dir': True,
                'use_metadata': True,
                'axis_forward': '-Z',
                'axis_up': 'Y'
            }
            
            # 执行导出
            bpy.ops.export_scene.fbx(**export_settings)
            
            # 恢复选择
            bpy.ops.object.select_all(action='DESELECT')
            for obj in original_selection:
                obj.select_set(True)
            if original_active:
                bpy.context.view_layer.objects.active = original_active
            
            return True, "FBX导出成功"
            
        except Exception as e:
            error_details = traceback.format_exc()
            log_manager.add('ERROR', f"FBX导出失败: {str(e)}", output_path, error_details)
            return False, f"FBX导出失败: {str(e)}"

# ============================================================================
# 4. 场景管理器
# ============================================================================
class ZW_SceneManager:
    
    @staticmethod
    def create_temp_scene():
        """创建临时场景用于转换"""
        original_scene = bpy.context.scene
        
        # 创建新场景
        temp_scene = bpy.data.scenes.new(name="Temp_Conversion_Scene")
        
        # 复制设置
        temp_scene.render.engine = original_scene.render.engine
        temp_scene.unit_settings.system = original_scene.unit_settings.system
        
        # 切换到新场景
        bpy.context.window.scene = temp_scene
        
        return original_scene, temp_scene
    
    @staticmethod
    def cleanup_temp_scene(temp_scene, original_scene):
        """清理临时场景"""
        try:
            # 删除所有对象
            if temp_scene:
                for obj in list(temp_scene.objects):
                    bpy.data.objects.remove(obj, do_unlink=True)
            
            # 删除孤立数据
            ZW_SceneManager._clean_orphan_data()
            
            # 删除临时场景
            if temp_scene and temp_scene.name in bpy.data.scenes:
                bpy.data.scenes.remove(temp_scene)
            
            # 切换回原场景
            if original_scene:
                bpy.context.window.scene = original_scene
                
        except Exception as e:
            log_manager.add('WARNING', f"清理场景时出错: {str(e)}")
    
    @staticmethod
    def _clean_orphan_data():
        """清理孤立的数据块"""
        for block_type in [bpy.data.meshes, bpy.data.materials, bpy.data.images]:
            for item in block_type:
                if item.users == 0:
                    try:
                        block_type.remove(item)
                    except:
                        pass

# ============================================================================
# 5. 操作符
# ============================================================================
class ZW_OT_batch_fbx_converter(Operator):
    """批量转换模型文件为FBX格式"""
    bl_idname = "zw.batch_fbx_converter"
    bl_label = "批量转换到FBX"
    bl_description = "批量转换文件夹中的所有模型文件为FBX格式"
    bl_options = {'REGISTER', 'UNDO'}
    
    directory: StringProperty(
        name="文件夹路径",
        description="选择要处理的文件夹",
        maxlen=1024,
        default="",
        subtype='DIR_PATH'
    )
    
    def execute(self, context):
        if not self.directory or not os.path.isdir(self.directory):
            self.report({'ERROR'}, "请选择有效的文件夹")
            return {'CANCELLED'}
        
        # 清空日志和结果
        log_manager.clear()
        context.scene.zw_conversion_results.clear()
        
        log_manager.add('INFO', f"开始处理文件夹: {self.directory}")
        
        # 获取所有文件
        file_list = self._get_files_to_process()
        
        if not file_list:
            log_manager.add('WARNING', "没有找到支持的模型文件")
            self.report({'WARNING'}, "没有找到支持的模型文件")
            return {'CANCELLED'}
        
        log_manager.add('INFO', f"找到 {len(file_list)} 个文件需要处理")
        
        success_count = 0
        fail_count = 0
        
        # 处理每个文件
        for i, input_path in enumerate(file_list):
            log_manager.add('INFO', f"处理文件 {i+1}/{len(file_list)}", input_path)
            
            result = self._convert_single_file(input_path, i)
            
            # 记录结果
            result_item = context.scene.zw_conversion_results.add()
            result_item.filepath = input_path
            result_item.success = result['success']
            result_item.message = result['message']
            result_item.output_path = result.get('output_path', '')
            
            if result['success']:
                success_count += 1
                log_manager.add('SUCCESS', result['message'], input_path)
            else:
                fail_count += 1
                log_manager.add('ERROR', result['message'], input_path, result.get('details', ''))
            
            # 更新UI
            self._update_ui(context)
        
        # 显示总结
        summary_msg = f"转换完成! 成功: {success_count}, 失败: {fail_count}, 总计: {len(file_list)}"
        log_manager.add('INFO', summary_msg)
        self.report({'INFO'}, summary_msg)
        
        # 保存输出目录到场景属性
        context.scene.zw_export_folder = os.path.join(self.directory, "导出FBX")
        
        return {'FINISHED'}
    
    def _get_files_to_process(self):
        """获取所有需要处理的文件"""
        file_list = []
        
        for root, dirs, files in os.walk(self.directory):
            # 跳过输出文件夹（修复：正确处理中文文件夹名）
            dirs[:] = [d for d in dirs if "导出FBX" not in d and "Exported_FBX" not in d]
            
            for filename in files:
                if ZW_FileProcessor.is_supported_format(filename):
                    full_path = os.path.join(root, filename)
                    file_list.append(full_path)
        
        return sorted(file_list, key=lambda x: x.lower())
    
    def _convert_single_file(self, input_path, index):
        """转换单个文件"""
        try:
            log_manager.add('DEBUG', f"开始转换: {os.path.basename(input_path)}", input_path)
            
            # 创建临时场景
            original_scene, temp_scene = ZW_SceneManager.create_temp_scene()
            
            # 导入文件
            import_success, import_message = ZW_FileProcessor.import_file(input_path)
            
            if not import_success:
                ZW_SceneManager.cleanup_temp_scene(temp_scene, original_scene)
                return {
                    'success': False,
                    'message': f"导入失败: {import_message}",
                    'details': import_message
                }
            
            # 检查是否有对象
            if not temp_scene.objects:
                ZW_SceneManager.cleanup_temp_scene(temp_scene, original_scene)
                return {
                    'success': False,
                    'message': "导入后没有找到任何对象"
                }
            
            # 生成输出路径（修改：全部放在同一文件夹）
            output_path = self._get_output_path(input_path, index)
            
            # 导出FBX
            export_success, export_message = ZW_FileProcessor.export_to_fbx(output_path, use_selection=False)
            
            # 清理
            ZW_SceneManager.cleanup_temp_scene(temp_scene, original_scene)
            
            if export_success:
                return {
                    'success': True,
                    'message': "转换成功",
                    'output_path': output_path
                }
            else:
                return {
                    'success': False,
                    'message': f"导出失败: {export_message}",
                    'details': export_message
                }
            
        except Exception as e:
            error_details = traceback.format_exc()
            log_manager.add('ERROR', f"转换异常: {str(e)}", input_path, error_details)
            
            return {
                'success': False,
                'message': f"转换异常: {str(e)}",
                'details': error_details
            }
    
    def _get_output_path(self, input_path, index):
        """生成输出路径 - 修改：全部放在同一文件夹"""
        # 构建输出目录（直接放在"导出FBX"文件夹，不创建子目录）
        output_dir = os.path.join(self.directory, "导出FBX")
        
        # 生成输出文件名
        input_name = os.path.splitext(os.path.basename(input_path))[0]
        output_name = f"{input_name}.fbx"
        
        # 如果文件名已存在，添加数字后缀避免覆盖
        counter = 1
        original_name = output_name
        while os.path.exists(os.path.join(output_dir, output_name)):
            base_name = os.path.splitext(original_name)[0]
            ext = os.path.splitext(original_name)[1]
            output_name = f"{base_name}_{counter}{ext}"
            counter += 1
            if counter > 100:  # 防止无限循环
                break
        
        return os.path.join(output_dir, output_name)
    
    def _update_ui(self, context):
        """更新UI"""
        try:
            for area in context.screen.areas:
                if area.type == 'VIEW_3D':
                    area.tag_redraw()
        except:
            pass
    
    def invoke(self, context, event):
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}

class ZW_OT_save_log_to_file(Operator):
    """保存日志到文件并自动打开删除"""
    bl_idname = "zw.save_log_to_file"
    bl_label = "保存日志到文件"
    bl_description = "将转换日志保存到输出文件夹的临时文件中并自动打开，3秒后删除"
    
    def execute(self, context):
        try:
            # 获取输出目录
            if hasattr(context.scene, 'zw_export_folder') and context.scene.zw_export_folder:
                output_dir = context.scene.zw_export_folder
            else:
                # 尝试从结果中获取输出路径
                if context.scene.zw_conversion_results:
                    for result in context.scene.zw_conversion_results:
                        if result.output_path:
                            output_dir = os.path.dirname(result.output_path)
                            break
                    else:
                        self.report({'WARNING'}, "没有找到输出目录")
                        return {'CANCELLED'}
                else:
                    self.report({'WARNING'}, "请先执行转换")
                    return {'CANCELLED'}
            
            # 确保输出目录存在
            if not os.path.exists(output_dir):
                os.makedirs(output_dir, exist_ok=True)
            
            # 保存日志到文件并打开
            success, message = log_manager.save_to_temp_file_and_open(output_dir)
            
            if success:
                self.report({'INFO'}, message)
            else:
                self.report({'ERROR'}, message)
                return {'CANCELLED'}
            
            return {'FINISHED'}
            
        except Exception as e:
            log_manager.add('ERROR', f"保存日志失败: {str(e)}")
            self.report({'ERROR'}, f"保存日志失败: {str(e)}")
            return {'CANCELLED'}

# ============================================================================
# 6. UI 面板（版本一：带日志功能）
# ============================================================================
class ZW_PT_batch_converter_main(Panel):
    """主面板 - 版本一：带日志功能"""
    bl_label = "批量FBX转换器"
    bl_idname = "ZW_PT_batch_converter_main"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "ZW_Blender"
    
    def draw(self, context):
        layout = self.layout
        
        # 批量转换部分
        box = layout.box()
        box.label(text="FBX批量转换", icon='EXPORT')
        
        col = box.column(align=True)
        col.scale_y = 2.0
        col.operator("zw.batch_fbx_converter", text="转FBX（文件夹）", icon='FILE_FOLDER')

        row = box.row(align=True)
        row.scale_y = 1
        row.operator("zw.save_log_to_file", text="打印日志", icon='TEXT')
                
        # 最近日志（可折叠）
        box = layout.box()
        row = box.row()
        
        # 可折叠控制
        show_logs = getattr(context.scene, 'zw_show_recent_logs', True)
        row.prop(context.scene, 'zw_show_recent_logs', 
                text="最近日志", 
                icon='TRIA_DOWN' if show_logs else 'TRIA_RIGHT',
                emboss=False)
        
        if show_logs:
            logs = log_manager.get_recent(10)
            
            if not logs:
                box.label(text="暂无日志", icon='INFO')
            else:
                for log_entry in logs:
                    row = box.row(align=True)
                    row.scale_y = 0.8
                    
                    # 根据日志级别显示不同图标
                    icon = 'INFO'
                    if log_entry['level'] == 'SUCCESS':
                        icon = 'CHECKMARK'
                    elif log_entry['level'] == 'ERROR':
                        icon = 'ERROR'
                    elif log_entry['level'] == 'WARNING':
                        icon = 'ERROR'
                    
                    row.label(text="", icon=icon)
                    
                    # 显示日志内容
                    if log_entry['filepath']:
                        filename = os.path.basename(log_entry['filepath'])
                        # 缩短显示，避免过长
                        display_msg = log_entry['message']
                        if len(display_msg) > 40:
                            display_msg = display_msg[:37] + "..."
                        row.label(text=f"{filename}: {display_msg}")
                    else:
                        msg = log_entry['message']
                        if len(msg) > 50:
                            msg = msg[:47] + "..."
                        row.label(text=msg)

# ============================================================================
# 7. 注册和初始化
# ============================================================================
classes = (
    ZW_ConversionResult,
    ZW_OT_batch_fbx_converter,
    ZW_OT_save_log_to_file,
    ZW_PT_batch_converter_main,
)

def register():
    """注册插件"""
    # 注册类
    for cls in classes:
        try:
            bpy.utils.register_class(cls)
        except Exception as e:
            print(f"注册类 {cls.__name__} 时出错: {e}")
    
    # 注册属性
    bpy.types.Scene.zw_conversion_results = CollectionProperty(type=ZW_ConversionResult)
    bpy.types.Scene.zw_show_recent_logs = BoolProperty(
        name="显示最近日志",
        default=True,
        description="展开或折叠最近日志显示"
    )
    bpy.types.Scene.zw_export_folder = StringProperty(
        name="导出文件夹",
        default="",
        description="最近导出的FBX文件所在文件夹"
    )
    
    # 设置日志回调
    def update_logs():
        try:
            for area in bpy.context.screen.areas:
                if area.type == 'VIEW_3D':
                    area.tag_redraw()
        except:
            pass
    
    log_manager.callback = update_logs
    
    # 注册退出时的清理函数
    atexit.register(lambda: log_manager._delete_temp_file())
    
    print("=" * 60)
    print("✅ ZW_Blender - 批量FBX转换器 v2.2.1 版本一")
    print("📋 版本特点:")
    print("  • 支持保存日志到输出文件夹")
    print("  • 自动打开日志文件并3秒后删除")
    print("  • 保留完整的日志显示功能")
    print("📁 使用方法:")
    print("  1. 点击'选择文件夹并转换'开始转换")
    print("  2. 转换完成后点击'保存日志到文件'")
    print("  3. 日志会自动打开并在3秒后删除")
    print("=" * 60)

def unregister():
    """注销插件"""
    # 清理临时文件
    try:
        log_manager._delete_temp_file()
    except:
        pass
    
    # 清理属性
    for prop_name in ['zw_conversion_results', 'zw_show_recent_logs', 'zw_export_folder']:
        if hasattr(bpy.types.Scene, prop_name):
            delattr(bpy.types.Scene, prop_name)
    
    # 注销类
    for cls in reversed(classes):
        try:
            bpy.utils.unregister_class(cls)
        except:
            pass
    
    print("ZW_Blender - 批量FBX转换器版本一已卸载")

# 脚本直接运行
if __name__ == "__main__":
    register()