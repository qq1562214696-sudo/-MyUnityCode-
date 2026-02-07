MacroScript QF_Tools_Lite
	ButtonText:"QF工具精简版"
	Category:"QF工具精简版" 
	internalCategory:"QF工具精简版" 
	Tooltip:"QF工具精简版"
	Icon:#("AddPreset",15)

(
    on execute do
    (
        -- 避免重复开启
        if QF_Tools_Floater != undefined and QF_Tools_Floater.open do closeRolloutFloater QF_Tools_Floater
        
        -- 建立弹窗
        QF_Tools_Floater = newRolloutFloater "QF工具精简版 1.0" 600 297
        addRollout theRollout QF_Tools_Floater
    )
)

-- ================== 全局变量和函数 ==================
prefixArray = #("H","L","HHair","HFace","HTop","HBottom","HShoe","LHair","LFace","LTop","LBottom","LShoe","HPet","LPet",\
"Head","Wing","Hand","FlyDecoration","Tail","LeftBracelet","RightBracelet","LeftAnklet","RightAnklet","Earring","Mask","Glass",\
"LeftHandChain","RightHandRing")

-- 新增：复制文本到剪切板函数
fn copyToClipboard str =
(
	try
	(
		local dotNetClass = dotNetClass "System.Windows.Forms.Clipboard"
		dotNetClass.SetText str
		return true
	)
	catch
	(
		try
		(
			setclipboardText str
			return true
		)
		catch
		(
			return false
		)
	)
)

-- 新增：复制max文件名到剪切板
fn copyMaxFileNameToClipboard =
(
	local fileName = ""
	if maxFileName != "" then
	(
		fileName = getFilenameFile maxFileName
		
		if copyToClipboard fileName then
		(
			print ("\n\n\n" + "已复制文件名到剪切板: " + fileName)
		)
		else
		(
			print ("\n\n\n" + "复制文件名到剪切板失败: " + fileName)
		)
	)
	else
	(
		print "当前文件未保存，无法复制文件名"
	)
	
	return fileName
)

-- 新增：强制显示侦听器窗口
fn showListenerWindow =
(
	try
	(
		actionMan.executeAction 0 "40472"  -- 侦听器窗口ID
	)
	catch
	(
		try
		(
			listener.Open()
		)
		catch
		(
			try
			(
				DOSCommand "max listener"
				format "正在打开侦听器窗口...\n"
			)
			catch
			(
				format "无法自动打开侦听器窗口，请手动按F11\n"
			)
		)
	)
)

-- 新增：安全格式化输出到侦听器
fn safeFormatToListener msgStr =
(
	try
	(
		showListenerWindow()
		format msgStr
		forceCompleteRedraw()
	)
	catch
	(
		try
		(
			print msgStr
		)
		catch()
	)
)

fn detectGender =
(
	local genderFolder = ""
	case of
	(
		(matchPattern maxFilePath pattern:"*_boy*"): genderFolder = "Male"
		(matchPattern maxFilePath pattern:"*_girl*"): genderFolder = "Female"
	)
	return genderFolder
)

fn getExportFolder prefix playerID genderFolder isPetNum assetsBase =
(
	local exportFolder = ""
	
	if isPetNum == 0 then
	(
		return ""
	)
	else if isPetNum <= 12 then
	(
		-- 普通角色部件 (H, L, HHair, LHair等)
		local partName = substring prefix 2 100
		local partFolder = ""
		case partName of
		(
			"Hair": partFolder = "01Hair"
			"Face": partFolder = "02Face"
			"Top": partFolder = "03Top"
			"Bottom": partFolder = "04Bottom"
			"Shoe": partFolder = "05Shoe"
		)
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\" + partFolder + "\\" + partName + "_" + playerID + "\\Model"
	)
	else if (isPetNum == 13 or isPetNum == 14) then
	(
		-- 宠物
		exportFolder = assetsBase + "\\ArtWork\\Pet\\Main\\Pet_" + playerID + "\\Model"
	)
	else if isPetNum == 15 then
	(
		-- Head（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\06Head\\Head_" + playerID + "\\Model"
	)
	else if isPetNum == 16 then
	(
		-- Wing（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\08Wing\\Wing_" + playerID + "\\Model"
	)
	else if isPetNum == 17 then
	(
		-- Hand（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\09Hand\\Hand_" + playerID + "\\Model"
	)
	else if isPetNum == 18 then
	(
		-- FlyDecoration（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\11FlyDecoration\\FlyDecoration_" + playerID + "\\Model"
	)
	else if isPetNum == 19 then
	(
		-- Tail（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\18Tail\\Tail_" + playerID + "\\Model"
	)
	else if isPetNum == 20 then
	(
		-- LeftBracelet（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\12LeftBracelet\\LeftBracelet_" + playerID + "\\Model"
	)
	else if isPetNum == 21 then
	(
		-- RightBracelet（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\13RightBracelet\\RightBracelet_" + playerID + "\\Model"
	)
	else if isPetNum == 22 then
	(
		-- LeftAnklet（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\14LeftAnklet\\LeftAnklet_" + playerID + "\\Model"
	)
	else if isPetNum == 23 then
	(
		-- RightAnklet（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\15RightAnklet\\RightAnklet_" + playerID + "\\Model"
	)
	else if isPetNum == 24 then
	(
		-- Earring（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\17Earring\\Earring_" + playerID + "\\Model"
	)
	else if isPetNum == 25 then
	(
		-- Mask（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\19Mask\\Mask_" + playerID + "\\Model"
	)
	else if isPetNum == 26 then
	(
		-- Glass（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\07Glass\\Glass_" + playerID + "\\Model"
	)
	else if isPetNum == 27 then
	(
		-- LeftHandChain（不分性别）
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\21LeftHandChain\\LeftHandChain_" + playerID + "\\Model"
	)
	else if isPetNum == 28 then
	(
		-- RightHandRing（分性别）
		if genderFolder == "" then return "" else
		exportFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\20RightHandRing\\RightHandRing_" + playerID + "\\Model"
	)
	
	return exportFolder
)

fn getTexFolder prefix playerID genderFolder isPetNum assetsBase =
(
	local texFolder = ""
	
	if isPetNum == 0 then
	(
		return ""
	)
	else if isPetNum <= 12 then
	(
		-- 普通角色部件
		local partName = substring prefix 2 100
		case partName of
		(
			"Hair": texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\01Hair\\Hair_" + playerID + "\\Textures"
			"Face": texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\02Face\\Face_" + playerID + "\\Textures"
			"Top": texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\03Top\\Top_" + playerID + "\\Textures"
			"Bottom": texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\04Bottom\\Bottom_" + playerID + "\\Textures"
			"Shoe": texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\05Shoe\\Shoe_" + playerID + "\\Textures"
		)
	)
	else if isPetNum == 15 then
	(
		-- Head
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\06Head\\Head_" + playerID + "\\Textures"
	)
	else if isPetNum == 16 then
	(
		-- Wing
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\08Wing\\Wing_" + playerID + "\\Textures"
	)
	else if isPetNum == 17 then
	(
		-- Hand
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\09Hand\\Hand_" + playerID + "\\Textures"
	)
	else if isPetNum == 18 then
	(
		-- FlyDecoration
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\11FlyDecoration\\FlyDecoration_" + playerID + "\\Textures"
	)
	else if isPetNum == 19 then
	(
		-- Tail
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\18Tail\\Tail_" + playerID + "\\Textures"
	)
	else if isPetNum == 20 then
	(
		-- LeftBracelet
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\12LeftBracelet\\LeftBracelet_" + playerID + "\\Textures"
	)
	else if isPetNum == 21 then
	(
		-- RightBracelet
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\13RightBracelet\\RightBracelet_" + playerID + "\\Textures"
	)
	else if isPetNum == 22 then
	(
		-- LeftAnklet
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\14LeftAnklet\\LeftAnklet_" + playerID + "\\Textures"
	)
	else if isPetNum == 23 then
	(
		-- RightAnklet
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\15RightAnklet\\RightAnklet_" + playerID + "\\Textures"
	)
	else if isPetNum == 24 then
	(
		-- Earring
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\17Earring\\Earring_" + playerID + "\\Textures"
	)
	else if isPetNum == 25 then
	(
		-- Mask
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\19Mask\\Mask_" + playerID + "\\Textures"
	)
	else if isPetNum == 26 then
	(
		-- Glass
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\07Glass\\Glass_" + playerID + "\\Textures"
	)
	else if isPetNum == 27 then
	(
		-- LeftHandChain
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Hang\\21LeftHandChain\\LeftHandChain_" + playerID + "\\Textures"
	)
	else if isPetNum == 28 then
	(
		-- RightHandRing
		if genderFolder == "" then return "" else
		texFolder = assetsBase + "\\ArtWork\\Avatar\\Character\\" + genderFolder + "\\20RightHandRing\\RightHandRing_" + playerID + "\\Textures"
	)
	
	return texFolder
)

fn ResetPivotFn Obj =
(
	--目标矩阵
	local target = matrix3 [1,0,0] [0,0,1] [0,-1,0] [0,0,0]

	--保存子物体的transforms矩阵
	local ChldTms = in coordSys target ( for Chld in Obj.children collect Chld.transform )

	--当前物体的transform矩阵
	local TmScale = scaleMatrix Obj.objectOffsetScale
	local TmRot = Obj.objectOffsetRot as matrix3
	local TmPos = transMatrix Obj.objectOffsetPos
	local TmOffset = TmScale * TmRot * TmPos

	--新的偏移transform矩阵
	TmOffset *= obj.transform * inverse target

	--应用矩阵
	Obj.transform = target

	--重置偏移
	Obj.objectOffsetPos = TmOffset.translation
	Obj.objectOffsetRot = TmOffset.rotation
	Obj.objectOffsetScale = TmOffset.scale

	--重制子物体的transform
	for i = 1 to Obj.children.count do Obj.children[i].transform = ChldTms[i] * inverse target * Obj.transform
)

-- ================== Rollout UI ==================
rollout theRollout "Q飞常用工具"
(
	edittext assetsPath pos:[68,7] width:445 height:21 text:"" readOnly:true
	button browseAssetsPath "浏览" pos:[520,7] width:60 height:21
		
	groupBox expGroup pos:[296,61] width:285 height:150
	button exportFbx "导出FBX(当前)" pos:[435,113] width:130 height:30	
	button btnRenameAllPoly "Mesh01_" pos:[307,178] width:125 height:25

	groupBox chkGroup pos:[5,61] width:285 height:150
	button selMultiEdgeFace "选择多边面" pos:[155, 76] width:125 height:30
	button selFilpNormalFace "选择法线翻转的面" pos:[155, 112] width:125 height:30
	button btnCheckFaceCount "面数检查" pos:[155,185] width:125 height:30

	groupBox fixGroup pos:[5,211] width:576 height:50
	button standardizeBtn "规范模型" pos:[10,224] width:280 height:29
	button exportAllBtn "导出（贴图+FBX）" pos:[300,224] width:280 height:29
		
	on theRollout open do
	(
		local configFilePath = GetDir #plugcfg + "\\" + "AvatorExpConfig.xml"
		if (doesFileExist configFilePath) do
		(
			local doc = dotNetObject "system.xml.xmlDocument"
			doc.Load configFilePath
			rootElem = doc.DocumentElement
			assetsPath.text = rootElem.GetAttribute "UnityAssetsPath"
		)
	)
	
	on browseAssetsPath pressed do
	(
		local theDialog = dotNetObject "System.Windows.Forms.FolderBrowserDialog"
		theDialog.Description = "请选择Unity Assets目录"
		local result = theDialog.showDialog()
		local assetsFolderPath = ""
		if result == result.OK then
		(
			assetsFolderPath = theDialog.SelectedPath
		)
		else
		(
			return()
		)
		if (findString assetsFolderPath "Assets") == undefined do
		(
			messagebox "请选择Unity工程的Assets目录！"
			return()
		)
		local configFilePath = GetDir #plugcfg + "\\" + "AvatorExpConfig.xml"
		local doc = dotNetObject "system.xml.xmlDocument"
		if (doesFileExist configFilePath) then
		(
			doc.Load configFilePath
			rootElem = doc.DocumentElement
			rootElem.SetAttribute "UnityAssetsPath" assetsFolderPath
		)
		else
		(
			local xmldecl = doc.CreateXmlDeclaration "1.0" "UTF-8" ""
			doc.AppendChild xmldecl
			
			local root = doc.CreateElement "Root"
			root.SetAttribute "UnityAssetsPath" assetsFolderPath
			doc.AppendChild root
		)
		doc.Save configFilePath
		assetsPath.text = assetsFolderPath
	)

	on selFilpNormalFace pressed do
	(
		if (selection.count == 0 or selection.count > 1) do
		(
			messagebox "请选择一个mesh物体"
			return()
		)
		
		local obj = selection[1]
		
		if (iskindof obj PolyMeshObject or iskindof obj Editable_Poly) then
		(
			local n_face = #() as array
			local cp = snapshot obj
			local nf = #{1..cp.numFaces}
			local g = for i in nf collect getFaceNormal cp i
			meshop.unifyNormals cp nf
			local x = for i in nf collect getFaceNormal cp i
			for i in nf do
			(
				if g[i] != x[i] do append n_face i
			)
			setFaceSelection cp n_face
			update cp
			convertToPoly cp
			local thefaces = polyop.getFaceSelection cp
			delete cp
			select obj
			max modify mode
			modPanel.setCurrentObject obj.baseObject
			subObjectLevel = 4
			polyop.setFaceSelection obj thefaces 
			update obj
		)
		else
		(
			messagebox "请选择Editable Poly类型的模型"
		)
	)
	
	on btnRenameAllPoly pressed do
	(
		undo "重命名所有Poly为Mesh01_" on
		(
			max unhide all
        
			local allPolys = for o in objects where classOf o == Editable_Poly collect o
        
			if allPolys.count == 0 do
			(
				messageBox "场景中没有找到任何 可编辑多边形 对象！" title:"提示"
				return()
			)
        
			for i = 1 to allPolys.count do
			(
				local newName = "Mesh" + (formattedPrint i format:"02d") + "_"
				allPolys[i].name = newName
			)
		)
	)

	on selMultiEdgeFace pressed do
	(
		if selection.count != 1 do
		(
			messageBox "请只选择一个物体！" title:"提示"
			return()
		)
    
		local obj = selection[1]
		if classOf obj.baseObject != Editable_Poly do
		(
			if canConvertTo obj Editable_Poly then
			(
				convertToPoly obj
			)
			else
			(
				messageBox "请选择可转换为 Editable Poly 的物体！" title:"不支持"
				return()
			)
		)
    
		polyOp.unhideAllFaces obj
    
		max modify mode
		modPanel.setCurrentObject obj
		subObjectLevel = 4
    
		local n_face = for f = 1 to (polyOp.getNumFaces obj) where (polyOp.getFaceVerts obj f).count > 4 collect f
    
		polyOp.setFaceSelection obj n_face
		update obj
    
		if n_face.count == 0 do
			messageBox "没有五边及以上面" title:"结果"
	)

	-- ================== 面数检查按钮逻辑 ==================
	on btnCheckFaceCount pressed do
	(
		-- 预设的面数限制表：根据文件名前缀确定限制
		local limitTable = #(
			#("Head",              1000),
			#("Wing",              2000),
			#("Hand",              1500),
			#("Tail",              1000),
			#("Mask",               100),
			#("Glass",              400),
			#("FlyDecoration",     2000),
			#("FloatDecoration",   2000),
			#("Earring",            300),
			#("LeftBracelet",       400),
			#("RightBracelet",      400),
			#("RightHandRing",      400),
			#("LeftHandChain",      500),
			#("LightRing",         2000)
		)
		
		-- 从当前max文件名中提取前缀
		local fileName = getFilenameFile maxFileName
		local nameParts = filterString fileName "_"
		local prefix = ""
		local prefixNum = 0
		
		if nameParts.count >= 1 then
		(
			-- 获取前缀，可能是"H"、"L"、"HHair"等
			prefix = nameParts[1]
			
			-- 如果是单字符前缀(H或L)，需要结合第二个部分
			if prefix == "H" or prefix == "L" and nameParts.count >= 2 then
			(
				prefix = nameParts[2] -- 比如Hair、Face、Top等
			)
			
			-- 检查是否在限制表中
			prefixNum = findItem (for item in limitTable collect item[1]) prefix
		)
		
		-- 收集所有以 "Mesh" 开头的对象（不区分大小写）
		local meshObjects = for o in objects where 
			matchPattern o.name pattern:"Mesh*" ignoreCase:true 
			collect o
		
		if meshObjects.count == 0 do
		(
			messageBox "场景中没有找到任何名称以 'Mesh' 开头的对象！" title:"面数检查结果"
			return()
		)
		
		local totalTris     = 0
		local msgDetails    = ""
		local objCount      = meshObjects.count
		
		-- 遍历所有 Mesh 对象，计算总面数
		for obj in meshObjects do
		(
			local triCount = 0
			
			-- 正确的 if-else 语法
			if isKindOf obj.baseObject Editable_Poly then
			(
				triCount = polyOp.getNumFaces obj.baseObject
			)
			else if isKindOf obj.baseObject Editable_Mesh then
			(
				triCount = meshOp.getNumFaces obj.baseObject
			)
			else if canConvertTo obj Editable_Poly then
			(
				-- 快照方式兼容 Skin、Morph 等修改器
				local snap = snapshotAsMesh obj
				triCount = getNumFaces snap
				delete snap
			)
			else
			(
				continue  -- 不支持的面数统计，跳过
			)
			
			totalTris += triCount
			
			-- 累加详细信息
			msgDetails += obj.name + "：" + (triCount as string) + " 面\n"
		)
		
		-- 格式化总面数（加千位分隔符）
		local formattedTotal = ""
		local strTotal = totalTris as string
		local len = strTotal.count
		
		if len > 3 then
		(
			local groups = #()
			local i = len
			while i > 0 do
			(
				local start = i - 2
				if start < 1 do start = 1
				local group = substring strTotal start (i - start + 1)
				append groups group
				i = start - 1
			)
			
			for i = groups.count to 1 by -1 do
			(
				formattedTotal += groups[i]
				if i > 1 do formattedTotal += ","
			)
		)
		else
		(
			formattedTotal = strTotal
		)
		
		-- 确定道具类型和面数限制
		local modelType = "未知类型"
		local faceLimit = 0
		local isOverLimit = false
		
		if prefixNum > 0 then
		(
			modelType = limitTable[prefixNum][1]
			faceLimit = limitTable[prefixNum][2]
			isOverLimit = totalTris > faceLimit
		)
		
		local finalMsg = ""

		finalMsg += "道具类型：" + modelType + "\n"
		finalMsg += "Mesh数量：" + (objCount as string) + " 个\n"
		finalMsg += "总三角面数：" + formattedTotal + "\n"
		
		if prefixNum > 0 then
		(
			finalMsg += "面数上限：" + (faceLimit as string) + "\n\n"
		)
		else
		(
			finalMsg += "面数上限：未知（未匹配到类型）\n\n"
		)
		
		-- 超过限制的醒目提示
		if isOverLimit then
		(
			finalMsg += "（超出 " + ((totalTris - faceLimit) as string) + " 面）\n\n"
		)

		
		-- 详细统计
		finalMsg += "---------\n"
		finalMsg += msgDetails
		
		-- 添加额外提示
		if prefixNum == 0 then
		(
			finalMsg += "\n提示：未识别到道具类型，请检查max文件名格式"
		)
		
		-- 显示结果
		messageBox finalMsg title:"面数检查完成" beep:isOverLimit
	)
	
	on standardizeBtn pressed do
	(
		local pureMaxFileName = getFilenameFile (maxFilePath + maxFileName)
		
		if matchPattern pureMaxFileName pattern:"*_fin" ignoreCase:true do
		(
			pureMaxFileName = substring pureMaxFileName 1 (pureMaxFileName.count - 4)
		)
		
		if maxFileName == "" do
		(
			messageBox "请先保存当前max文件！"
			return()
		)
		
		for i in 1 to 24 do
		(
			meditMaterials[i] = Standard()
		)
		
		local firstMat = Standard()
		firstMat.name = pureMaxFileName + "_D"
		firstMat.showInViewport = true
		
		local diffuseMapPath = maxFilePath + pureMaxFileName + "_D.png"
		local alphaMapPath = maxFilePath + pureMaxFileName + "_A.png"
		
		if doesFileExist diffuseMapPath then
		(
			local diffuseMap = Bitmaptexture()
			diffuseMap.filename = filenameFromPath diffuseMapPath
			diffuseMap.name = pureMaxFileName + "_D"
			firstMat.diffuseMap = diffuseMap
			firstMat.diffuseMapEnable = true
		)
		else
		(
			messageBox ("未找到漫反射贴图: " + pureMaxFileName + "_D.png" + "\n请确保贴图文件与max文件在同一目录下。")
		)
		
		if doesFileExist alphaMapPath then
		(
			local alphaMap = Bitmaptexture()
			alphaMap.filename = filenameFromPath alphaMapPath
			alphaMap.name = pureMaxFileName + "_A"
			firstMat.opacityMap = alphaMap
			firstMat.opacityMapEnable = true
		)

		meditMaterials[1] = firstMat

		undo on
		(
			local meshCount = 0
			for obj in objects where matchPattern obj.name pattern:"Mesh*" ignoreCase:true do
			(
				try
				(
					obj.material = firstMat
					meshCount += 1
				)
				catch()
			)

			for obj in objects where matchPattern obj.name pattern:"Mesh*" ignoreCase:true do
			(
				if obj.parent != undefined do
				(
					obj.parent = undefined
				)
			)

			local allPolyObjects = for obj in objects where classOf obj == Editable_Poly collect obj
			
			for obj in allPolyObjects do
			(
				polyop.setMapSupport obj 0 false
				polyop.setMapSupport obj 2 false
				polyop.setMapSupport obj 3 false
			)
			
			if allPolyObjects.count > 0 do
			(
				local oldSelection = selection as array
				
				select allPolyObjects
				
				macros.run "Animation Tools" "DeleteSelectedAnimation"
				
				select oldSelection
			)
			
			for obj in allPolyObjects do
			(
				ResetPivotFn obj
				obj.scale = [1.0,1.0,1.0]
			)

			local hasRoot  = false
			local hasBip   = false
			local rootObj  = undefined
			local bipObj   = undefined
			local nodeObj  = undefined

			for o in objects do
			(
				local n = o.name
				if matchPattern n pattern:"Root*"  do (hasRoot = true;  if rootObj  == undefined do rootObj  = o)
				if matchPattern n pattern:"Bip*"   do (hasBip  = true;  if bipObj   == undefined do bipObj   = o)
				if matchPattern n pattern:"Node*"  and nodeObj == undefined do nodeObj = o
			)

			local linkTarget = undefined

			if hasRoot and not hasBip and nodeObj == undefined then
			(
				linkTarget = rootObj
			)
			else if not hasRoot and nodeObj != undefined and not hasBip then
			(
				linkTarget = nodeObj
			)
			else if hasRoot and hasBip then
			(
				linkTarget = bipObj
			)

			if linkTarget != undefined do
			(
				local allMeshes = for o in objects where matchPattern o.name pattern:"Mesh*" collect o
				for m in allMeshes do
				(
					if m != linkTarget and m.parent != linkTarget do
					(
						m.parent = linkTarget
					)
				)
			)
		)
	)

	on exportFbx pressed do
	(
		if (doesFileExist assetsPath.text) == false do
		(
			Messagebox "请在地址栏指定正确的Unity工程Assets目录"
			return()
		)

		if maxFileName == "" do
		(
			messageBox "当前场景尚未保存，请先保存 .max 文件！"
			return()
		)

		FbxExporterSetParam "Animation" false
		FbxExporterSetParam "Cameras" false
		FbxExporterSetParam "Lights" false

		local expLog = "导出FBX结果：\n"
		
		max unhide all
		
		local exportObjs = #()
		for obj in objects do
		(
			local objName = obj.name
			if matchPattern objName pattern:"Bip*" or
			matchPattern objName pattern:"Mesh*" or
			matchPattern objName pattern:"Root*" or
			matchPattern objName pattern:"Node*" or
			matchPattern objName pattern:"Bone*" do
			(
				append exportObjs obj
			)
		)
		
		if exportObjs.count == 0 do
		(
			messageBox "没有找到任何名称以 Bip、Mesh、Root、Bone 开头的对象！\n导出将无法继续。" title:"警告"
			return()
		)
		
		selectionSets["Export"] = exportObjs
		
		local objNameArray = #()
		if selectionSets["Export"] != undefined do
		(
			for obj in selectionSets["Export"] do append objNameArray obj.name
		)
		
		clearSelection()
		local selectedCount = 0
		for objName in objNameArray do
		(
			local obj = getNodeByName objName
			if obj != undefined do
			(
				selectMore obj
				selectedCount += 1
			)
		)
		
		if selectedCount == 0 do
		(
			expLog += "错误：选择集中没有找到任何有效对象\n"
			return()
		)
		
		local fileFullName = maxFilePath + maxFileName
		local fileName = getFilenameFile fileFullName
		local nameArray = filterString fileName "_"
		local prefix = nameArray[1]
		local playerID = nameArray[2]

		local isPetNum = findItem prefixArray prefix
		local genderFolder = detectGender()
		local fbxFileName = prefix + "_" + playerID
		
		if isPetNum == 0 then
		(
			expLog += "错误：未知的前缀 " + prefix + "\n"
			return()
		)
		
		local exportFolder = getExportFolder prefix playerID genderFolder isPetNum assetsPath.text
		
		if exportFolder == "" do
		(
			expLog += "错误：无法确定导出路径（可能缺少性别标识）\n"
			return()
		)
		
		if (doesFileExist exportFolder) == false do makeDir exportFolder
		
		local fbxPath = exportFolder + "\\" + fbxFileName + ".fbx"
		local success = exportFile fbxPath #noPrompt selectedOnly:true using:FBXEXP
		
		if success == true then
		(
			expLog += "FBX导出成功: " + fbxFileName + ".fbx\n"
		)
		else
		(
			expLog += "FBX导出失败：导出 " + fbxFileName + ".fbx 失败\n"
		)
		
		if selectionSets["Export"] != undefined do
		(
			select selectionSets["Export"]
		)

		-- 新增：导出结束后复制max文件名到剪切板
		copyMaxFileNameToClipboard()
		
		-- 修改：使用新的安全格式化函数
		if expLog != "导出FBX结果：\n" then
		(
			safeFormatToListener expLog
		)
		else
		(
			messageBox "当前文件 FBX 导出完成，无错误记录。" title:"完成"
		)
	)

	on exportAllBtn pressed do
	(
		if (doesFileExist assetsPath.text) == false do
		(
			Messagebox "请在地址栏指定正确的Unity工程Assets目录"
			return()
		)
		
		if maxFileName == "" do
		(
			messagebox "当前场景未保存，请先保存文件"
			return()
		)
		
		max unhide all
		
		local exportObjs = #()
		for obj in objects do
		(
			local objName = obj.name
			if matchPattern objName pattern:"Bip*" or
			matchPattern objName pattern:"Mesh*" or
			matchPattern objName pattern:"Root*" or
			matchPattern objName pattern:"Node*" or
			matchPattern objName pattern:"Bone*" do
			(
				append exportObjs obj
			)
		)
		
		if exportObjs.count == 0 do
		(
			messageBox "没有找到任何名称以 Bip、Mesh、Root、Bone 开头的对象！\n导出将无法继续。" title:"警告"
			return()
		)
		
		selectionSets["Export"] = exportObjs
		
		local objNameArray = #()
		if selectionSets["Export"] != undefined do
		(
			for obj in selectionSets["Export"] do append objNameArray obj.name
		)

		FbxExporterSetParam "Animation" false
		FbxExporterSetParam "Cameras" false
		FbxExporterSetParam "Lights" false

		local expLog = "导出结果：\n"
		
		-- 导出贴图
		local fileFullName = maxFilePath + maxFileName
		local fileName = getFilenameFile fileFullName
		local nameArray = filterString fileName "_"
		local prefix = nameArray[1]
		local playerID = nameArray[2]
				
		local imageArray = getFiles (maxFilePath + "\\*.png")
		
		if imageArray.count == 0 then
		(
			expLog += "该目录下没有PNG贴图文件\n"
		)
		else
		(
			local isPetNum = findItem prefixArray prefix
			local genderFolder = detectGender()
			
			if isPetNum != 0 then
			(
				local texFolder = getTexFolder prefix playerID genderFolder isPetNum assetsPath.text
				
				if texFolder != "" do
				(
					if (doesFileExist texFolder) == false do makeDir texFolder
					
					for imgFile in imageArray do
					(
						local imgFileName = filenameFromPath imgFile
						local targetPath = texFolder + "\\" + imgFileName
						try
						(
							copyFile imgFile targetPath
							expLog += "贴图导出成功: " + imgFileName + "\n"
						)
						catch
						(
							expLog += "贴图导出失败: 导出 " + imgFileName + " 失败\n"
						)
					)
				)
			)
		)
		
		-- 导出FBX
		clearSelection()
		local selectedCount = 0
		for objName in objNameArray do
		(
			local obj = getNodeByName objName
			if obj != undefined do
			(
				selectMore obj
				selectedCount += 1
			)
		)
		
		if selectedCount == 0 then
		(
			expLog += "错误：选择集中没有找到任何有效对象\n"
		)
		else
		(
			local isPetNum = findItem prefixArray prefix
			local genderFolder = detectGender()
			local fbxFileName = prefix + "_" + playerID
			
			if isPetNum == 0 then
			(
				expLog += "错误：未知的前缀 " + prefix + "\n"
			)
			else
			(
				local exportFolder = getExportFolder prefix playerID genderFolder isPetNum assetsPath.text
				
				if exportFolder == "" do
				(
					expLog += "错误：无法确定导出路径（可能缺少性别标识）\n"
				)
				
				if exportFolder != "" do
				(
					if (doesFileExist exportFolder) == false do makeDir exportFolder
					
					local fbxPath = exportFolder + "\\" + fbxFileName + ".fbx"
					local success = exportFile fbxPath #noPrompt selectedOnly:true using:FBXEXP
					
					if success == true then
					(
						expLog += "FBX导出成功: " + fbxFileName + ".fbx\n"
					)
					else
					(
						expLog += "FBX导出失败：导出 " + fbxFileName + ".fbx 失败\n"
					)
				)
			)
		)
		
		if selectionSets["Export"] != undefined do
		(
			select selectionSets["Export"]
		)
		
		-- 新增：导出结束后复制max文件名到剪切板
		copyMaxFileNameToClipboard()
		
		-- 修改：使用新的安全格式化函数
		if expLog != "导出结果：\n" then
		(
			safeFormatToListener expLog
		)
		else
		(
			messagebox "导出完成！"
		)
	)
)

theNewFloater = newRolloutFloater "QF工具精简版 1.0" 600 297
addRollout theRollout theNewFloater