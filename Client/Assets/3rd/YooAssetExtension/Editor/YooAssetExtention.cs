using YooAsset.Editor;
using System.IO;

public class YooAssetExtention
{
    [DisplayName("Lua文件")]
    public class PackLua : IPackRule
    {
        PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
        {
            string bundleName = "lua";
            PackRuleResult result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IPackRule.IsRawFilePackRule()
        {
            return true;
        }
        
        public class CollectAtlas : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                return Path.GetExtension(data.AssetPath) == ".spriteatlas";
            }
        }
    }
}
