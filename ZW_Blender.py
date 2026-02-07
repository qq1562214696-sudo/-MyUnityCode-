bl_info = {
    "name": "ZW_Blender: 批量FBX转换器 - 优化版",
    "author": "ZW",
    "version": (1, 1, 0),
    "blender": (3, 0, 0),
    "location": "3D视图 > 右侧面板 > ZW_Blender",
    "description": "批量转换模型文件为FBX格式，专为3ds Max优化",
    "category": "Import-Export",
    "doc_url": "",
    "tracker_url": "",
}

import bpy
import os
import json
import time
import traceback
from pathlib import Path
from bpy.props import StringProperty, CollectionProperty, BoolProperty
from bpy.types import Operator, Panel, OperatorFileListElement, PropertyGroup

# 格式配置 - 简化版
FORMAT_CONFIG = {
    '.obj': {
        'operator': 'import_scene.obj',
        'type': 'import',
    },
    '.fbx': {
        'operator': 'import_scene.fbx',
        'type': 'import',
    },
    '.blend': {
        'operator': 'wm.append',
        'type': 'append',
    },
    '.gltf': {
        'operator': 'import_scene.gltf',
        'type': 'import',
    },
    '.glb': {
        'operator': 'import_scene.gltf',
        'type': 'import',
    },
    '.dae': {
        'operator': 'wm.collada_import',
        'type': 'import',
    },
    '.3ds': {
        'operator': 'import_scene.autodesk_3ds',
        'type': 'import',
    },
    '.ply': {
        'operator': 'import_mesh.ply',
        'type': 'import',
    },
    '.stl': {
        'operator': 'import_mesh.stl',
        'type': 'import',
    },
}

class ZW_ConversionLog:
    """日志管理器"""
    def __init__(self):
        self.logs = []
        self.start_time = time.time()
    
    def add(self, level, message, filepath=""):
        log_entry = {
            'time': time.strftime("%H:%M:%S"),
            'level': level,
            'message': message,
            'filepath': filepath
        }
        self.logs.append(log_entry)
        print(f"[{log_entry['time']}] {level}: {message}")
    
    def clear(self):
        self.logs = []
        self.start_time = time.time()
    
    def get_summary(self):
        total = len([l for l in self.logs if l['level'] in ['SUCCESS', 'ERROR']])
        success = len([l for l in self.logs if l['level'] == 'SUCCESS'])
        errors = len([l for l in self.logs if l['level'] == 'ERROR'])
        elapsed = time.time() - self.start_time
        
        return {
            'total': total,
            'success': success,
            'errors': errors,
            'elapsed': f"{elapsed:.2f}秒"
        }

# 全局日志实例
log_manager = ZW_ConversionLog()

class ZW_ConversionResult(PropertyGroup):
    filepath: StringProperty(name="原始文件")
    success: BoolProperty(name="成功")
    message: StringProperty(name="消息")
    output_path: StringProperty(name="输出路径")

class ZW_OT_batch_fbx_converter(Operator):
    """批量转换模型文件为FBX格式 - 优化版"""
    bl_idname = "zw.batch_fbx_converter"
    bl_label = "批量转换到FBX"
    bl_options = {'REGISTER', 'UNDO'}
    
    directory: StringProperty(
        name="文件夹路径",
        description="选择要处理的文件夹",
        maxlen=1024,
        default="",
        subtype='DIR_PATH'
    )
    
    def execute(self, context):
        if not self.directory:
            self.report({'ERROR'}, "请选择文件夹")
            return {'CANCELLED'}
        
        # 清空日志
        log_manager.clear()
        log_manager.add('INFO', f"开始处理文件夹: {self.directory}")
        
        # 获取所有要处理的文件
        file_list = self.get_files_to_process()
        
        if not file_list:
            log_manager.add('WARNING', "没有找到支持的模型文件")
            self.report({'WARNING'}, "没有找到支持的模型文件")
            return {'CANCELLED'}
        
        log_manager.add('INFO', f"找到 {len(file_list)} 个文件需要处理")
        
        # 初始化结果记录
        context.scene.zw_conversion_results.clear()
        
        success_count = 0
        fail_count = 0
        
        for i, (input_path, rel_path) in enumerate(file_list):
            log_manager.add('INFO', f"处理文件 {i+1}/{len(file_list)}: {os.path.basename(input_path)}")
            
            # 转换单个文件
            result = self.convert_single_file(context, input_path, rel_path, i)
            
            # 记录结果
            result_item = context.scene.zw_conversion_results.add()
            result_item.filepath = input_path
            result_item.success = result['success']
            result_item.message = result['message']
            result_item.output_path = result.get('output_path', '')
            
            if result['success']:
                success_count += 1
                log_manager.add('SUCCESS', f"转换成功: {os.path.basename(input_path)}")
            else:
                fail_count += 1
                log_manager.add('ERROR', f"转换失败: {os.path.basename(input_path)} - {result['message']}")
        
        # 显示总结
        summary = log_manager.get_summary()
        log_manager.add('INFO', f"转换完成! 总共: {summary['total']}, 成功: {summary['success']}, 失败: {summary['errors']}, 耗时: {summary['elapsed']}")
        
        # 显示输出目录
        output_dirs = set()
        for item in context.scene.zw_conversion_results:
            if item.success and item.output_path:
                output_dirs.add(os.path.dirname(item.output_path))
        
        for dir_path in output_dirs:
            log_manager.add('INFO', f"输出到: {dir_path}")
        
        self.report({'INFO'}, f"转换完成: {success_count} 成功, {fail_count} 失败")
        return {'FINISHED'}
    
    def get_files_to_process(self):
        """获取文件夹中所有支持的模型文件"""
        file_list = []
        
        if not os.path.isdir(self.directory):
            return []
        
        # 递归获取所有文件
        for root, dirs, files in os.walk(self.directory):
            for filename in files:
                if self.is_supported_format(filename):
                    full_path = os.path.join(root, filename)
                    rel_path = os.path.relpath(full_path, self.directory)
                    file_list.append((full_path, rel_path))
        
        # 按文件名排序，便于跟踪进度
        file_list.sort(key=lambda x: x[0])
        
        return file_list
    
    def is_supported_format(self, filename):
        """检查是否为支持的格式"""
        ext = os.path.splitext(filename)[1].lower()
        return ext in FORMAT_CONFIG
    
    def convert_single_file(self, context, input_path, rel_path, index):
        """转换单个文件"""
        try:
            log_manager.add('INFO', f"开始转换: {os.path.basename(input_path)}", input_path)
            
            # 保存当前场景
            original_scene = context.scene
            
            # 创建新场景用于导入
            temp_scene = bpy.data.scenes.new(name=f"Temp_Conv_{index}")
            context.window.scene = temp_scene
            
            # 设置场景单位（3ds Max兼容）
            temp_scene.unit_settings.system = 'METRIC'
            temp_scene.unit_settings.scale_length = 1.0
            
            # 清空新场景
            self.clean_scene(temp_scene)
            
            # 尝试导入
            import_success = self.import_file(input_path)
            
            if not import_success:
                self.cleanup_temp_scene(temp_scene, original_scene)
                return {'success': False, 'message': '导入失败'}
            
            # 检查是否有导入的对象
            if not temp_scene.objects:
                self.cleanup_temp_scene(temp_scene, original_scene)
                return {'success': False, 'message': '导入后场景为空'}
            
            # 准备输出路径
            output_path = self.get_output_path(input_path, rel_path)
            
            # 导出为FBX
            export_success = self.export_to_fbx(temp_scene, output_path, input_path)
            
            # 清理
            self.cleanup_temp_scene(temp_scene, original_scene)
            
            if export_success:
                return {
                    'success': True, 
                    'message': '转换成功',
                    'output_path': output_path
                }
            else:
                return {'success': False, 'message': '导出失败'}
            
        except Exception as e:
            error_msg = str(e)
            log_manager.add('ERROR', f"转换异常: {error_msg}", input_path)
            
            # 确保恢复原场景
            try:
                context.window.scene = original_scene
            except:
                pass
            
            return {'success': False, 'message': f'异常: {error_msg}'}
    
    def import_file(self, filepath):
        """导入文件"""
        ext = os.path.splitext(filepath)[1].lower()
        
        if ext not in FORMAT_CONFIG:
            return False
        
        try:
            if ext == '.obj':
                bpy.ops.import_scene.obj(
                    filepath=filepath,
                    use_split_objects=True,
                    use_split_groups=True,
                    use_image_search=True
                )
                
            elif ext == '.fbx':
                bpy.ops.import_scene.fbx(filepath=filepath)
                
            elif ext == '.blend':
                # 只导入网格、骨架和空物体
                with bpy.data.libraries.load(filepath, link=False) as (data_from, data_to):
                    data_to.objects = [name for name in data_from.objects if name]
                
                # 链接到场景
                for obj in data_to.objects:
                    if obj and obj.type in {'MESH', 'ARMATURE', 'EMPTY'}:
                        bpy.context.collection.objects.link(obj)
                
            elif ext in ['.gltf', '.glb']:
                bpy.ops.import_scene.gltf(filepath=filepath)
                
            elif ext == '.dae':
                bpy.ops.wm.collada_import(filepath=filepath)
                
            elif ext == '.3ds':
                bpy.ops.import_scene.autodesk_3ds(filepath=filepath)
                
            elif ext == '.ply':
                bpy.ops.import_mesh.ply(filepath=filepath)
                
            elif ext == '.stl':
                bpy.ops.import_mesh.stl(filepath=filepath)
                
            else:
                return False
            
            return True
            
        except Exception as e:
            log_manager.add('ERROR', f"导入失败: {str(e)[:100]}", filepath)
            return False
    
    def export_to_fbx(self, scene, output_path, source_path):
        """导出为FBX（3ds Max兼容）"""
        try:
            # 确保输出目录存在
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            
            # 选择所有对象
            bpy.ops.object.select_all(action='SELECT')
            
            # 检查是否有材质需要处理
            has_materials = False
            for obj in scene.objects:
                if hasattr(obj, 'material_slots') and obj.material_slots:
                    has_materials = True
                    break
            
            log_manager.add('INFO', f"导出FBX到: {os.path.basename(output_path)}", source_path)
            
            # 导出设置 - 重点优化
            bpy.ops.export_scene.fbx(
                filepath=output_path,
                use_selection=True,
                
                # 只导出必要的类型
                object_types={'EMPTY', 'ARMATURE', 'MESH'},
                
                # 3ds Max兼容设置
                global_scale=1.0,
                apply_scale_options='FBX_SCALE_NONE',
                axis_forward='-Z',
                axis_up='Y',
                
                # 网格设置
                mesh_smooth_type='EDGE',
                use_mesh_modifiers=True,
                use_subsurf=False,
                
                # 材质和纹理 - 确保嵌入纹理信息
                bake_space_transform=False,
                
                # 材质处理
                use_mesh_edges=False,
                use_tspace=False,
                
                # 动画 - 不导出
                bake_anim=False,
                bake_anim_use_all_bones=False,
                bake_anim_use_nla_strips=False,
                bake_anim_use_all_actions=False,
                
                # 嵌入纹理 - 保留纹理路径信息但不嵌入文件
                embed_textures=False,
                path_mode='AUTO',
                
                # 其他优化
                use_custom_props=False,
                add_leaf_bones=False,
                primary_bone_axis='Y',
                secondary_bone_axis='X',
                use_armature_deform_only=True,
                armature_nodetype='NULL',
            )
            
            return True
            
        except Exception as e:
            log_manager.add('ERROR', f"导出失败: {str(e)[:100]}", source_path)
            return False
    
    def get_output_path(self, input_path, rel_path):
        """生成输出路径：同级目录/文件夹名_FBX_Exports/..."""
        # 获取输入文件夹的名称
        input_folder = os.path.basename(self.directory)
        
        # 构建输出基础路径
        parent_dir = os.path.dirname(self.directory)
        output_base = os.path.join(parent_dir, f"{input_folder}_FBX_Exports")
        
        # 如果有子文件夹结构，保持结构
        if os.path.dirname(rel_path):
            output_dir = os.path.join(output_base, os.path.dirname(rel_path))
        else:
            output_dir = output_base
        
        # 确保目录存在
        os.makedirs(output_dir, exist_ok=True)
        
        # 生成输出文件名
        input_name = os.path.splitext(os.path.basename(input_path))[0]
        output_name = f"{input_name}.fbx"
        
        return os.path.join(output_dir, output_name)
    
    def clean_scene(self, scene):
        """清理场景中的所有对象"""
        # 解除所有对象的链接
        for obj in list(scene.objects):
            scene.collection.objects.unlink(obj)
        
        # 删除所有对象
        for obj in list(bpy.data.objects):
            if obj.users == 0:
                bpy.data.objects.remove(obj)
        
        # 清理孤立的材质和纹理
        for block in [bpy.data.materials, bpy.data.images, bpy.data.meshes, bpy.data.armatures]:
            for item in block:
                if item.users == 0:
                    block.remove(item)
        
        # 强制释放内存
        bpy.ops.wm.memory_statistics()
    
    def cleanup_temp_scene(self, temp_scene, original_scene):
        """清理临时场景并恢复原场景"""
        # 恢复到原场景
        bpy.context.window.scene = original_scene
        
        # 清理临时场景
        if temp_scene:
            # 移除临时场景中的所有对象
            self.clean_scene(temp_scene)
            
            # 删除临时场景
            if temp_scene.name in bpy.data.scenes:
                bpy.data.scenes.remove(temp_scene)
        
        # 强制垃圾回收
        bpy.ops.wm.memory_statistics()
    
    def invoke(self, context, event):
        # 打开文件夹选择对话框
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}

class ZW_PT_batch_fbx_converter(Panel):
    """批量FBX转换面板"""
    bl_label = "批量FBX转换"
    bl_idname = "ZW_PT_batch_fbx_converter"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "ZW_Blender"
    
    def draw(self, context):
        layout = self.layout
        
        # 说明
        box = layout.box()
        box.label(text="使用方法:", icon='INFO')
        box.label(text="1. 点击下方按钮选择文件夹")
        box.label(text="2. 自动处理所有子文件夹中的模型文件")
        box.label(text="3. 输出到: 同级目录/文件夹名_FBX_Exports")
        
        # 主要按钮
        layout.separator()
        row = layout.row()
        row.scale_y = 2.0
        op = row.operator("zw.batch_fbx_converter", 
                         text="选择文件夹并批量转换", 
                         icon='EXPORT')
        
        # 支持的格式
        layout.separator()
        box = layout.box()
        box.label(text="支持的格式:", icon='FILE_3D')
        
        # 显示支持的格式
        formats_row = box.row()
        col1 = formats_row.column()
        col2 = formats_row.column()
        
        formats = ['.obj', '.fbx', '.blend', '.gltf', '.glb', '.dae', '.3ds', '.ply', '.stl']
        for i, fmt in enumerate(formats):
            if i % 2 == 0:
                col1.label(text=f"• {fmt}")
            else:
                col2.label(text=f"• {fmt}")
        
        # 3ds Max兼容说明
        layout.separator()
        box = layout.box()
        box.label(text="3ds Max兼容设置:", icon='IMPORT')
        
        col = box.column(align=True)
        col.label(text="• 轴向: Y向上，-Z向前")
        col.label(text="• 单位: 米制")
        col.label(text="• 平滑: 边缘平滑组")
        col.label(text="• 材质: 保留贴图路径")
        
        # 处理日志
        if log_manager.logs:
            layout.separator()
            box = layout.box()
            box.label(text="处理日志:", icon='TEXT')
            
            summary = log_manager.get_summary()
            row = box.row()
            row.label(text=f"总计: {summary['total']}", icon='LINENUMBERS_ON')
            row.label(text=f"成功: {summary['success']}", icon='CHECKMARK')
            row.label(text=f"失败: {summary['errors']}", icon='X')
            row.label(text=f"耗时: {summary['elapsed']}", icon='TIME')
            
            # 显示最近的日志（最多10条）
            box.separator()
            recent_logs = log_manager.logs[-10:]  # 只显示最近10条
            
            for log_entry in recent_logs:
                row = box.row(align=True)
                
                # 时间
                row.label(text=log_entry['time'], icon='TIME')
                
                # 图标
                if log_entry['level'] == 'SUCCESS':
                    row.label(text="", icon='CHECKMARK')
                elif log_entry['level'] == 'ERROR':
                    row.label(text="", icon='X')
                elif log_entry['level'] == 'WARNING':
                    row.label(text="", icon='ERROR')
                else:
                    row.label(text="", icon='INFO')
                
                # 消息（截断过长的消息）
                message = log_entry['message']
                if len(message) > 40:
                    message = message[:37] + "..."
                row.label(text=message)
            
            # 显示详细错误
            error_logs = [l for l in log_manager.logs if l['level'] == 'ERROR']
            if error_logs and len(error_logs) > 0:
                box.separator()
                box.label(text="详细错误:", icon='ERROR')
                
                # 只显示前5个错误
                for i, log_entry in enumerate(error_logs[:5]):
                    row = box.row(align=True)
                    filename = os.path.basename(log_entry['filepath']) if log_entry['filepath'] else "未知"
                    row.label(text=f"{filename}: {log_entry['message']}")
            
            # 清除日志按钮
            box.separator()
            row = box.row()
            row.operator("zw.clear_logs", text="清除日志", icon='TRASH')

class ZW_OT_clear_logs(Operator):
    """清除日志"""
    bl_idname = "zw.clear_logs"
    bl_label = "清除日志"
    
    def execute(self, context):
        log_manager.clear()
        context.scene.zw_conversion_results.clear()
        self.report({'INFO'}, "日志已清除")
        return {'FINISHED'}

# 定义所有要注册的类
classes = (
    ZW_ConversionResult,
    ZW_OT_batch_fbx_converter,
    ZW_OT_clear_logs,
    ZW_PT_batch_fbx_converter,
)

def register():
    # 注册类
    for cls in classes:
        bpy.utils.register_class(cls)
    
    # 注册场景属性
    bpy.types.Scene.zw_conversion_results = CollectionProperty(type=ZW_ConversionResult)
    
    print("=" * 70)
    print("✅ ZW_Blender - 批量FBX转换器 (优化版) 安装成功！")
    print("=" * 70)
    print("📁 功能特点:")
    print("  • 只处理文件夹，递归搜索所有子文件夹")
    print("  • 输出到: 同级目录/文件夹名_FBX_Exports")
    print("  • 3ds Max兼容: Y向上，-Z向前")
    print("  • 内存优化: 每个文件处理完后清理场景")
    print("  • 详细日志: 实时显示处理进度和错误")
    print("  • 性能优化: 适合批量处理上百个文件")
    print("=" * 70)
    print("📁 位置: 3D视图右侧面板 > ZW_Blender选项卡")
    print("=" * 70)

def unregister():
    # 删除场景属性
    if hasattr(bpy.types.Scene, 'zw_conversion_results'):
        del bpy.types.Scene.zw_conversion_results
    
    # 注销类
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)
    
    print("ZW_Blender - 批量FBX转换器插件已卸载")

# 这允许脚本直接在文本编辑器中运行
if __name__ == "__main__":
    register()