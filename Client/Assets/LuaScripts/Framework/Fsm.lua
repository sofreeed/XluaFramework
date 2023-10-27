--- @class Fsm 有限状态机。
--- 每个状态是一个 lua table。其中必须包含
--- * Name 字段，获取状态名。在同一 Fsm 实例中以此作为状态的唯一标识。
--- * OnEnter 方法，进入该状态的回调。
--- * OnLeave 方法，离开该状态的回调。
--- 可选包含
--- * OnCreate 方法，状态机创建完成的回调。
--- * OnDestroy 方法，状态机销毁时的回调。
--- * OnTick 方法，状态机轮询时发给当前状态的回调。
--- 所有 OnXXX 方法均包含至少两个参数，依次是所属状态本身和当前 Fsm 对象。
local Fsm = BaseClass("Fsm")

--- 轮询模式。
Fsm.TickModes = {
    --- 不轮询。
    None = 0,
    --- 外部手动轮询。通过调用 Fsm.Tick 方法。
    External = 1,
    --- 全局 Update 轮询。
    UpdateBeat = 2,
    --- 全局 LateUpdate 轮询。
    LateUpdateBeat = 3,
}

local tickModeValues = {}
for k, v in pairs(Fsm.TickModes) do
    if tickModeValues[v] then
        error("TickModes have duplicate values.")
    end
    tickModeValues[v] = k
end

--- @param self Fsm
local function DebugLog(self, msg, ...)
    if Config.Debug_Fsm then
        Logger.LogInfo(string.format("[Fsm %s] %s", tostring(self.name), msg), ...)
    end
end

--- @param self Fsm
local function Tick(self)
    local currentState = self.currentState
    if not currentState then
        return
    end
    local onTick = currentState.OnTick
    if type(onTick) ~= 'function' then
        return
    end
    onTick(currentState, self)
end

--- @param self Fsm
--- @param tickMode number
local function InitTick(self, tickMode)
    if tickMode == self.TickModes.UpdateBeat then
        self.tickContainer = UpdateBeat
    elseif tickMode == self.TickModes.LateUpdateBeat then
        self.tickContainer = LateUpdateBeat
    end
    if self.tickContainer then
        self.tickListener = self.tickContainer:CreateListener(Tick, self)
        self.tickContainer:AddListener(self.tickListener)
    end
end

--- @param self Fsm
local function DeInitTick(self)
    if not self.tickContainer then
        return
    end
    self.tickContainer:RemoveListener(self.tickListener)
    self.tickContainer = nil
    self.tickListener = nil
end

--- 初始化。
--- @param name string 状态机名，主要用于调试。可空。
--- @param stateTableList table 状态列表，数组形式。
--- @param tickMode number 轮询模式。取自 Fsm.TickModes 的值。
--- @param owner any 状态机的持有者。
--- @param initStateName string 初始状态名。
function Fsm:__init(name, stateTableList, tickMode, owner, initStateName)
    if name ~= nil and type(name) ~= 'string' then
        error("Invalid name.")
    end
    if not tickModeValues[tickMode] then
        error(string.format("Illegal tick mode: %s.", tickMode))
    end
    if type(stateTableList) ~= 'table' or #stateTableList <= 0 then
        error("Invalid state table list.")
    end
    local stateTableMap = {}
    for i, stateTable in ipairs(stateTableList) do
        if type(stateTable) ~= 'table' then
            error(string.format("State table at index %d is invalid.", i))
        end
        local stateName = stateTable.Name
        if type(stateName) ~= 'string' then
            error(string.format("State table at index %d has no invalid Name.", i))
        end
        if stateTableMap[stateName] then
            error(string.format("Duplicate state name [%s].", stateName))
        end
        if type(stateTable.OnEnter) ~= 'function' then
            error(string.format("State table [%s] has no invalid OnEnter method.", stateName))
        end
        if type(stateTable.OnLeave) ~= 'function' then
            error(string.format("State table [%s] has no invalid OnLeave method.", stateName))
        end
        stateTableMap[stateName] = stateTable
    end

    if type(initStateName) ~= 'string' then
        error("Invalid init state name.")
    end

    local initStateTable = stateTableMap[initStateName]
    if not initStateTable then
        error(string.format("Init state [%s] doesn't exist.", initStateName))
    end

    self.name = name
    self.tickMode = tickMode
    self.owner = owner
    self.stateTableMap = stateTableMap
    for _, stateTable in pairs(self.stateTableMap) do
        local onCreate = stateTable.OnCreate
        if type(onCreate) == 'function' then
            onCreate(stateTable, self)
        end
    end
    self:SwitchState(initStateName)
    InitTick(self, tickMode)
    DebugLog(self, "[init] tickMode=%s, initStateName=%s", tickModeValues[tickMode], initStateName)
end

--- 设置状态切换前的回调。
--- @param callback function 三个参数：当前 Fsm 实例、旧状态名、新状态名。
function Fsm:SetStateWillChangeCallback(callback)
    if callback ~= nil and type(callback) ~= 'function' then
        error("Invalid callback.")
    end
    self.onStateWillChange = callback
end

--- 设置状态切换后的回调。
--- @param callback function 三个参数：当前 Fsm 实例、旧状态名、新状态名。
function Fsm:SetStateDidChangeCallback(callback)
    if callback ~= nil and type(callback) ~= 'function' then
        error("Invalid callback.")
    end
    self.onStateDidChange = callback
end

--- 获取持有者。
function Fsm:GetOwner()
    return self.owner
end

--- 获取名称。
function Fsm:GetName()
    return self.name
end

--- 切换状态。
--- @param stateName string 目标状态名。
function Fsm:SwitchState(stateName)
    if type(stateName) ~= 'string' then
        error("Invalid state name.")
    end
    local stateTable = self.stateTableMap[stateName]
    if not stateTable then
        error(string.format("State [%s] doesn't exist.", stateName))
    end
    local currentState = self.currentState
    if currentState == stateTable then
        return
    end
    local oldStateName = self:GetCurrentStateName()
    DebugLog(self, "[SwitchState] Pre: %s -> %s", oldStateName, stateName)
    local onStateWillChange = self.onStateWillChange
    if onStateWillChange then
        onStateWillChange(self, oldStateName, stateName)
    end
    if self.currentState then
        self.currentState:OnLeave(self)
    end
    self.currentState = stateTable
    stateTable:OnEnter(self)
    local onStateDidChange = self.onStateDidChange
    if onStateDidChange then
        onStateDidChange(self, oldStateName, stateName)
    end
    DebugLog(self, "[SwitchState] Post: %s -> %s", oldStateName, stateName)
end

--- 获取当前状态名。
function Fsm:GetCurrentStateName()
    local currentState = self.currentState
    if currentState then
        return currentState.Name
    end
    return nil
end

--- 获取当前状态。
function Fsm:GetCurrentState()
    return self.currentState
end

--- 手动轮询。
function Fsm:Tick()
    if self.tickMode ~= self.TickModes.External then
        error("You cannot tick the FSM with a tick mode [%s]", tickModeValues[self.tickMode])
    end
    Tick(self)
end

--- 销毁。
function Fsm:__delete()
    DeInitTick(self)
    self.onStateDidChange = nil
    self.onStateWillChange = nil
    if self.currentState then
        self.currentState:OnLeave(self)
    end
    for _, stateTable in pairs(self.stateTableMap) do
        local onDestroy = stateTable.OnDestroy
        if type(onDestroy) == 'function' then
            onDestroy(stateTable, self)
        end
    end
    self.stateTableMap = nil
    self.owner = nil
    self.currentState = nil
end

return Fsm