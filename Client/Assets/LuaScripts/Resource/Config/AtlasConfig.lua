--[[
-- added by wsh @ 2018-01-08
-- 图集配置
--]]

local AtlasConfig = {
	Comm = {
		Name = "Comm",
		--AtlasPath = "UI/Atlas/Comm",
		AtlasPath = "Assets/AssetsPackage/UI/Atlas/Comm/"
	},
	Group = {
		Name = "Group",
		PackagePath = "Assets/AssetsPackage/UI/Atlas/Comm",
	},
	Hyper = {
		Name = "Hyper",
		AtlasPath = "Assets/AssetsPackage/UI/Atlas/Hyper",
	},
	Login = {
		Name = "Login",
		AtlasPath = "Assets/AssetsPackage/UI/Atlas/Login",
	},
	Role = {
		Name = "Role",
		AtlasPath = "Assets/AssetsPackage/UI/Atlas/Role",
	},
}

return ConstClass("AtlasConfig", AtlasConfig)