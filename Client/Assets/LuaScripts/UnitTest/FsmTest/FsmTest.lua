--状态机持有者
local FsmTest = {}

--状态机类
local FsmClass = require("Common.Fsm")
--定义3个状态
local FsmState_Open = require("UnitTest.FsmTest.FsmState_Open")
local FsmState_Push = require("UnitTest.FsmTest.FsmState_Push")
local FsmState_Close = require("UnitTest.FsmTest.FsmState_Close")
--3个状态放入table
local fsmStates = {FsmState_Open, FsmState_Push, FsmState_Close};

--创建状态机实例，参数：状态机名字、状态列表、Tick(Update)模式、Owner、初始状态
local fsm = FsmClass.New("myFsm", fsmStates, FsmClass.TickModes.External, FsmTest, "FsmState_Open");

--设置状态切换前的回调
fsm:SetStateWillChangeCallback(
        function(fsm, oldStateName, newStateName)
            Logger.LogError("SetStateWillChangeCallback...")
        end
);

--设置状态切换后的回调。
fsm:SetStateDidChangeCallback(
        function(fsm, oldStateName, newStateName)
            Logger.LogError("SetStateDidChangeCallback...")
        end
);

--切换状态
fsm:SwitchState("FsmState_Push");
--手动Tick
fsm:Tick();

--持有者
local owner = fsm:GetOwner();
--状态机名字
local fsmName = fsm:GetName();

--当前状态和名字
local currentState = fsm:GetCurrentState();
local currentStateName = fsm:GetCurrentStateName();

fsm:Delete();