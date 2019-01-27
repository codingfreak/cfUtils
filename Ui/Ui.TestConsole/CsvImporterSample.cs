namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
    using System.Linq;

    using Logic.Csv;

    public class CsvImporterSample
    {
        #region properties

        /// <summary>
        /// activate button DWord
        /// </summary>
        [Property("activate_button_DW")]
        public double ActivateButton { get; set; }

        /// <summary>
        /// actual measurement of el. filling sensor
        /// </summary>
        [Property("actual_fs_measurement")]
        public double ActualFillingSensorMeasurement { get; set; }

        /// <summary>
        /// button status Word
        /// </summary>
        [Property("button_status_W")]
        public double ButtonStatus { get; set; }

        /// <summary>
        /// Ch/h possible x10 INT
        /// </summary>
        [Property("Ch_h_p")]
        public double ChargesPerHourPossible { get; set; }

        /// <summary>
        /// Ch/h real x10 INT
        /// </summary>
        [Property("Ch_h_r")]
        public double ChargesPerHourReal { get; set; }

        /// <summary>
        /// charge counter
        /// </summary>
        [Property("charge_counter")]
        public double ChargeCounter { get; set; }

        /// <summary>
        /// scaled pressure of discharger
        /// </summary>
        [Property("discharger_pressure_actual")]
        public double DischargerActualValue { get; set; }

        /// <summary>
        /// actual value of discharger position
        /// </summary>
        [Property("discharger_twist_actual")]
        public double DischargerPositionActualValue { get; set; }

        /// <summary>
        /// teached value of discharger position
        /// </summary>
        [Property("discharger_twist_teached")]
        public double DischargerPositionTeachedValue { get; set; }

        /// <summary>
        /// FC disorder code
        /// </summary>
        [Property("FC_disorder_code")]
        public double FillingCycleDisorderCode { get; set; }

        /// <summary>
        /// FC power infeed received and scaled
        /// </summary>
        [Property("FC_inverter_power")]
        public double FillingCycleInverterPower { get; set; }

        /// <summary>
        /// FC current motor modul received and scaled
        /// </summary>
        [Property("FC_motor_current")] 
        public double FillingCycleMotorCurrent { get; set; }

        /// <summary>
        /// FC power motor modul received and scaled
        /// </summary>
        [Property("FC_motor_power")] 
        public double FillingCycleMotorPower { get; set; }

        /// <summary>
        /// FC speed motor modul received and scaled
        /// </summary>
        [Property("FC_motor_speed")] 
        public double FillingCycleMotorSpeed { get; set; }

        /// <summary>
        /// FC motor temperature
        /// </summary>
        [Property("FC_motor_temperature")] 
        public double FillingCycleMotorTemperature { get; set; }

        /// <summary>
        /// FC torque motor modul received and scaled
        /// </summary>
        [Property("FC_motor_torque")] 
        public double FillingCycleMotorTorque { get; set; }

        /// <summary>
        /// FC voltage motor modul received and scaled
        /// </summary>
        [Property("FC_motor_voltage")] 
        public double FillingCycleMotorVoltage { get; set; }

        /// <summary>
        /// feedback message DWord 1
        /// </summary>
        [Property("feedback_message_DW1")] 
        public double FirstFeedbackMessage { get; set; }

        /// <summary>
        /// feedback message DWord 2
        /// </summary>
        [Property("feedback_message_DW2")] 
        public double SecondFeedbackMessage { get; set; }

        /// <summary>
        /// feedback message word 3
        /// </summary>
        [Property("feedback_message_W3")] 
        public double ThirdFeedbackMessage { get; set; }

        /// <summary>
        /// set filling level by HMI
        /// </summary>
        [Property("filling_level_set")] 
        public double HmiFillingLevelSet { get; set; }

        /// <summary>
        /// charging valve process value
        /// </summary>
        [Property("filling_valve_PV")] 
        public double ChargingValveProcessValue { get; set; }

        /// <summary>
        /// charging valve set point
        /// </summary>
        [Property("filling_valve_SP")] 
        public double ChargingValveSetpoint { get; set; }

        /// <summary>
        /// maximum filling level
        /// </summary>
        [Property("maximum_filling_level")] 
        public double MaximumFillingLevel { get; set; }

        /// <summary>
        /// operation counter
        /// </summary>
        [Property("operation_counter")]
        public double OperationCounter { get; set; }

        /// <summary>
        /// proof which senors has shut down the filling process
        /// </summary>
        [Property("proof_end_filling")]
        public double SensorFillingProcessShutdownProof { get; set; }

        /// <summary>
        /// value syrup actual
        /// </summary>
        [Property("RC_actual")] 
        public double SyrupValveActualValue { get; set; }

        /// <summary>
        /// value syrup max
        /// </summary>
        [Property("RC_max")] 
        public double SyrupValveMaxValue { get; set; }

        /// <summary>
        /// filling level switching point at  end of filling
        /// </summary>
        [Property("seen_level_filling_end")] 
        public double FillingLevelSwitchingPointEndOfFilling { get; set; }

        /// <summary>
        /// set speed discharging
        /// </summary>
        [Property("set_discharge_speed")] 
        public double SetFirstDischargingSpeed { get; set; }

        /// <summary>
        /// set speed discharging 2
        /// </summary>
        [Property("set_discharge_speed_2")] 
        public double SetSecondDischargingSpeed { get; set; }

        /// <summary>
        /// set speed filling
        /// </summary>
        [Property("set_filling_speed")] 
        public double SetFillingSpeed { get; set; }

        /// <summary>
        /// set speed intermediate spinning
        /// </summary>
        [Property("set_inter_spin_speed")] 
        public double SetIntermediateSpinSpeed { get; set; }

        /// <summary>
        /// set speed spinning
        /// </summary>
        [Property("set_spin_speed")] 
        public double SetSpinningSpeed { get; set; }

        /// <summary>
        /// set switching point el. filling sensor
        /// </summary>
        [Property("set_value_fs")] 
        public double SetFillingSensorSwitchingPoint { get; set; }

        /// <summary>
        /// state message Byte 1
        /// </summary>
        [Property("state_message_1")] 
        public double FirstStateMessage { get; set; }

        /// <summary>
        /// state message Byte 2
        /// </summary>
        [Property("state_message_2")] 
        public double SecondStateMessage { get; set; }

        /// <summary>
        /// state message Byte 3
        /// </summary>
        [Property("state_message_3")] 
        public double ThirdStateMessage { get; set; }

        /// <summary>
        /// T delay disorder hard sugar x10
        /// </summary>
        [Property("T_hard_sugar")]
        public double HardSugarDisorderDelay { get; set; }

        /// <summary>
        /// T sugar get water to reset disorder x10
        /// </summary>
        [Property("T_sugar_get_water")]
        public double ResetOrderWaterRetrieveDelay { get; set; }

        /// <summary>
        /// T1 cycle time monitoring x10
        /// </summary>
        [Property("T1")]
        public double CycleTime { get; set; }

        /// <summary>
        /// T1 charging time monitoring backward
        /// </summary>
        [Property("T1_backward")]
        public double BackwardChargingMonitoringTime { get; set; }

        /// <summary>
        /// T10 delay steam after syrup x10
        /// </summary>
        [Property("T10")]
        public double SteamDelayAfterSyrup { get; set; }

        /// <summary>
        /// T11 duration steam after syrup x10
        /// </summary>
        [Property("T11")]
        public double SteamDurationAfterSyrup { get; set; }

        /// <summary>
        /// T12 duration intermediate spinning speed x10
        /// </summary>
        [Property("T12")]
        public double IntermediateSpinningSpeedDuration { get; set; }

        /// <summary>
        /// T13 duration spinning speed x10
        /// </summary>
        [Property("T13")]
        public double SpinningSpeedDuration { get; set; }

        /// <summary>
        /// T14 duration screen washing x10
        /// </summary>
        [Property("T14")]
        public double ScreenWashingDuration { get; set; }

        /// <summary>
        /// T15 discharger time swing in (E/R version) / discharger on screen upper side (L version) x10
        /// </summary>
        [Property("T15")]
        public double DischargerTimeUpperSide { get; set; }

        /// <summary>
        /// T16 discharger on screen lower side (L version) x10
        /// </summary>
        [Property("T16")]
        public double DischargerTimeLowerSide { get; set; }

        /// <summary>
        /// T17 delay feed duct rinsing x10
        /// </summary>
        [Property("T17")]
        public double FeedDuctRisingDelay { get; set; }

        /// <summary>
        /// T18 duration feed duct rinsing x10
        /// </summary>
        [Property("T18")]
        public double FeedDuctRisingDuration { get; set; }

        /// <summary>
        /// T19 delay syrup separation green x10
        /// </summary>
        [Property("T19")]
        public double SyrupSeparationGreenDelay { get; set; }

        /// <summary>
        /// T2 filling time monitoring x10
        /// </summary>
        [Property("T2")]
        public double FillingTime { get; set; }


        /// <summary>
        /// T2 filling time monitoring backward
        /// </summary>
        [Property("T2_backward")]
        public double BackwardFillingMonitoringTime { get; set; }

        /// <summary>
        /// T20 waiting time acceleration interlog x10
        /// </summary>
        [Property("T20")]
        public double InterlogAccelerationWaitingTime { get; set; }

        /// <summary>
        /// T21 waiting time discharge interlog x10
        /// </summary>
        [Property("T21")]
        public double InterlogDischargeWaitingTime { get; set; }

        /// <summary>
        /// T22 delay close charging flap x10
        /// </summary>
        [Property("T22")]
        public double ChargingFlapCloseDelay { get; set; }

        /// <summary>
        /// T25 delay discharger rinsing x10
        /// </summary>
        [Property("T25")]
        public double DischargerRisingDelay { get; set; }

        /// <summary>
        /// T26 duration discharger rinsing x10
        /// </summary>
        [Property("T26")]
        public double DischargerRisingDuration { get; set; }

        /// <summary>
        /// T3 delay syrup separation white x10
        /// </summary>
        [Property("T3")]
        public double SyrupSeparationDelay { get; set; }

        /// <summary>
        /// T30 duration cleaning cycle x10
        /// </summary>
        [Property("T30")]
        public double CleaningCycleDuration { get; set; }

        /// <summary>
        /// T31 break x10
        /// </summary>
        [Property("T31")]
        public double BreakDuration { get; set; }

        /// <summary>
        /// T32 duration 2. discharger rinsing x10
        /// </summary>
        [Property("T32")]
        public double SecondDischargerRisingDuration { get; set; }

        /// <summary>
        /// T33 delay discharging speed 2 x10
        /// </summary>
        [Property("T33")]
        public double DischargingSpeedDelay { get; set; }

        /// <summary>
        /// T34 duration of dry spin after screen washing x10
        /// </summary>
        [Property("T34")]
        public double DrySpinAfterScreenWashingDuration { get; set; }

        /// <summary>
        /// T4 delay syrup washing x10
        /// </summary>
        [Property("T4")]
        public double SyrupWashingDelay { get; set; }

        /// <summary>
        /// T40a delay output A x10
        /// </summary>
        [Property("T40a")]
        public double OutputADelay { get; set; }

        /// <summary>
        /// T40b delay output B x10
        /// </summary>
        [Property("T40b")]
        public double OutputBDelay { get; set; }

        /// <summary>
        /// T40C delay ouput C x10
        /// </summary>
        [Property("T40c")]
        public double OutputCDelay { get; set; }

        /// <summary>
        /// T41a duration output A x10
        /// </summary>
        [Property("T41a")]
        public double OutputADuration { get; set; }

        /// <summary>
        /// T41b duration output B x10
        /// </summary>
        [Property("T41b")]
        public double OutputBDuration { get; set; }

        /// <summary>
        /// T41c duration output C x10
        /// </summary>
        [Property("T41c")]
        public double OutputCDuration { get; set; }

        /// <summary>
        /// T42 time accelerate to spinning speed [sec]
        /// </summary>
        [Property("T42")]
        public double SpinningSpeedAccelerationTime { get; set; }

        /// <summary>
        /// T43 time brake to discharging speed [sec]
        /// </summary>
        [Property("T43")]
        public double BrakeToDischargingSpeedTime { get; set; }

        /// <summary>
        /// T44 delay cleaning ring line x10
        /// </summary>
        [Property("T44")]
        public double CleanRingLineDelay { get; set; }

        /// <summary>
        /// T45 duration cleaning ring line x10
        /// </summary>
        [Property("T45")]
        public double CleanRingLineDuration { get; set; }

        /// <summary>
        /// T46 time discharging [sec]
        /// </summary>
        [Property("T46")]
        public double DischargingTime { get; set; }

        /// <summary>
        /// T47 filling time [sec]
        /// </summary>
        [Property("T47")]
        public double FillingTimeInSeconds { get; set; }

        /// <summary>
        /// T48 time accelerate to intermediate spinning speed [sec]
        /// </summary>
        [Property("T48")]
        public double AccelarationToIntermediateSpinningSpeedTime { get; set; }

        /// <summary>
        /// T5 duration syrup washing x10
        /// </summary>
        [Property("T5")]
        public double SyrupWashingDuration { get; set; }

        /// <summary>
        /// T50 duration open syrup valve by hand x10
        /// </summary>
        [Property("T50")]
        public double ManualSyrupValvaOpenDuration { get; set; }

        /// <summary>
        /// T51 duration open steam valve by hand x10
        /// </summary>
        [Property("T51")]
        public double ManualSteamValveOpenDuration { get; set; }

        /// <summary>
        /// T52 duration open water valve by hand x10
        /// </summary>
        [Property("T52")]
        public double ManualWaterValveOpenDelay { get; set; }

        /// <summary>
        /// T53 duration open ring line by hand x10
        /// </summary>
        [Property("T53")]
        public double ManualRingOpenDuration { get; set; }

        /// <summary>
        /// T6a delay water washing 1 x10
        /// </summary>
        [Property("T6a")]
        public double FirstWaterWashingDelay { get; set; }

        /// <summary>
        /// T6b delay water washing 2 x10
        /// </summary>
        [Property("T6b")]
        public double SecondWaterWashingDelay { get; set; }

        /// <summary>
        /// T6c delay water washing 3 x10
        /// </summary>
        [Property("T6c")]
        public double ThirdWaterWashingDelay { get; set; }

        /// <summary>
        /// T7a duration water washing 1 x1
        /// </summary>
        [Property("T7a")]
        public double FirstWaterWashingDuration { get; set; }

        /// <summary>
        /// T7b duration water washing 2 x10
        /// </summary>
        [Property("T7b")]
        public double SecondWaterWashingDuration { get; set; }

        /// <summary>
        /// T7c duration water washing 3 x10
        /// </summary>
        [Property("T7c")]
        public double ThirdWaterWashingDuration { get; set; }

        /// <summary>
        /// T8 delay steam washing x10
        /// </summary>
        [Property("T8")]
        public double SteamWashingDelay { get; set; }

        /// <summary>
        /// T9 duration steam washing x10
        /// </summary>
        [Property("T9")]
        public double SteamWashingDuration { get; set; }

        /// <summary>
        /// The time when the measurement occured on the device.
        /// </summary>
        [Property("time")]
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// TL1 cycle time monitoring running x10
        /// </summary>
        [Property("TL1")]
        public double RunningCycleTime { get; set; }

        /// <summary>
        /// TL2 filling time monitoring running x10
        /// </summary>
        [Property("TL2")]
        public double RunningFillingTime { get; set; }

        /// <summary>
        /// highest vibration analog signal
        /// </summary>
        [Property("vibration")]
        public double Vibration { get; set; }

        [Property("warnung_error_1")] 
        public double WarningError1 { get; set; }

        [Property("warnung_error_10")] 
        public double WarningError10 { get; set; }

        [Property("warnung_error_11")] 
        public double WarningError11 { get; set; }

        [Property("warnung_error_12")] 
        public double WarningError12 { get; set; }

        [Property("warnung_error_2")] 
        public double WarningError2 { get; set; }

        [Property("warnung_error_3")] 
        public double WarningError3 { get; set; }

        [Property("warnung_error_4")] 
        public double WarningError4 { get; set; }

        [Property("warnung_error_5")] 
        public double WarningError5 { get; set; }

        [Property("warnung_error_6")] 
        public double WarningError6 { get; set; }

        [Property("warnung_error_7")] 
        public double WarningError7 { get; set; }

        [Property("warnung_error_8")] 
        public double WarningError8 { get; set; }

        [Property("warnung_error_9")] 
        public double WarningError9 { get; set; }

        #endregion
    }
}