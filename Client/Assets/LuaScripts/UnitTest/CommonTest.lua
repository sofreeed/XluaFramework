
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

local function StringAndByte()
    coroutine.waitforframes(1);
    CS.Logger.LogError("11111111111111111111111111");

    --例子1
    local str = '258'
    local ascii_str = string.pack('<H', str)
    --H占2个字节 并且以小端存储，故存储的格式应该是下面的样子
    -- 00000010 00000001
    print(ascii_str)

    --string.byte() 将一个ASCII字符还原回常用字符
    local byte1 = ascii_str:byte(1)
    --数据的低位，  输出 byte1 = 	2
    local byte2 = ascii_str:byte(2)
    --数据的高位 输出 byte2 = 	1

    print('byte1 = ',byte1)
    print('byte2 = ',byte2)

    --所以，需要将 258 还原，只需要高位乘上 256 即可，即
    local real_value = byte1 + byte2*256
    --Logger.LogError(real_value)
    --输出 258

    CS.Logger.LogError("22222222222222222222222222");
end

local function Run()
    --测试string和byte
    StringAndByte();
    
    --测试可变参数
    VariableParamTest(nil,1,nil,3,nil)      
    
    --原生Pack和Unpack的使用
    OriginalPackTest(nil,1,nil,3,nil)
end

return {
    Run = Run
}