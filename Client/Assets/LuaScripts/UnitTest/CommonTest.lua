
-- 变参的使用
local function VariableParamTest(...)
    --使用方式1：用多参接
    local a, b, c, d, e = ...;

    --使用方式2：转化成tab，在对table进行操作，有nil值问题
    local tab = { ... };
    print("长度1：" .. #tab)        --2
    --这种方式也不对
    local tablen = select("#", tab)
    print("长度2：" .. tablen)      --1

    --使用方式3：使用Select进行操作，避免nil值问题
    local paramLen = select("#", ...)
    print("长度3：" .. paramLen)    --5

    --使用方式4：使用SafePack,方法内还是使用了Select，把长度放到了tab.n上
    --所以取tab的值使用SafeUnpack，他是用tab.n取的
    local safePackTab = SafePack(...)
    --遍历的时候会有n，错误
    for k, v in pairs(safePackTab) do
        --print(k .. "-------------：" .. v)
    end
    --应使用
    local a1, b1, c1, d1, e1 = SafeUnpack(safePackTab);
end

--原生Pack和Unpack的使用
local function OriginalPackTest(...)
    local tab = table.pack(...)
    --输出：
    --2-------------：1
    --4-------------：3
    --n-------------：5
    --table.pack原始是塞了一个n，SafePack和这个一样
    for k, v in pairs(tab) do
        --print(k .. "-------------：" .. v)
    end

    --这种情况会错，结果 --> nil 1 nil nil nil
    --local a1, b1, c1, d1, e1 = table.unpack(tab);
    --正确，结果 --> nil,1,nil,3,nil
    local a1, b1, c1, d1, e1 = table.unpack(tab, 1, tab.n);
end

local function Run()
    --测试可变参数
    VariableParamTest(nil,1,nil,3,nil)      
    
    --原生Pack和Unpack的使用
    OriginalPackTest(nil,1,nil,3,nil)
end

return {
    Run = Run
}