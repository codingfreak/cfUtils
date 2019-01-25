namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
    using System.Linq;

    using Logic.Csv;

    public class CsvImporterSample
    {
        #region properties


        //public double ActivateButton { get; set; }

        //public double actual_fs_measurement { get; set; }

        //public double button_status_W { get; set; }

        //public double Ch_h_p { get; set; }

        //public double Ch_h_r { get; set; }

        //public double charge_counter { get; set; }

        //public double discharger_pressure_actual { get; set; }

        //public double discharger_twist_actual { get; set; }

        //public double discharger_twist_teached { get; set; }

        //public double FC_disorder_code { get; set; }

        //public double FC_inverter_power { get; set; }

        //public double FC_motor_current { get; set; }

        //public double FC_motor_power { get; set; }

        //public double FC_motor_speed { get; set; }

        //public double FC_motor_temperature { get; set; }

        //public double FC_motor_torque { get; set; }

        //public double FC_motor_voltage { get; set; }

        //public double feedback_message_DW1 { get; set; }

        //public double feedback_message_DW2 { get; set; }

        //public double feedback_message_W3 { get; set; }

        //public double filling_level_set { get; set; }

        //public double filling_valve_PV { get; set; }

        //public double filling_valve_SP { get; set; }

        //public double maximum_filling_level { get; set; }

        //public double operation_counter { get; set; }

        //public double proof_end_filling { get; set; }

        //public double RC_actual { get; set; }

        //public double RC_max { get; set; }

        //public double seen_level_filling_end { get; set; }

        //public double set_discharge_speed { get; set; }

        //public double set_discharge_speed_2 { get; set; }

        //public double set_filling_speed { get; set; }

        //public double set_inter_spin_speed { get; set; }

        //public double set_spin_speed { get; set; }

        //public double set_value_fs { get; set; }

        //public double state_message_1 { get; set; }

        //public double state_message_2 { get; set; }

        //public double state_message_3 { get; set; }

        //public double T_hard_sugar { get; set; }

        //public double T_sugar_get_water { get; set; }

        /// <summary>
        /// T1 cycle time monitoring x10
        /// </summary>
        [Property("T1")]
        public double CycleTime { get; set; }

        //public double T1_backward { get; set; }

        //public double T10 { get; set; }

        //public double T11 { get; set; }

        //public double T12 { get; set; }

        //public double T13 { get; set; }

        //public double T14 { get; set; }

        //public double T15 { get; set; }

        //public double T16 { get; set; }

        //public double T17 { get; set; }

        //public double T18 { get; set; }

        //public double T19 { get; set; }

        /// <summary>
        /// T2 filling time monitoring x10
        /// </summary>
        [Property("T2")]
        public double FillingTime { get; set; }

        //public double T2_backward { get; set; }

        //public double T20 { get; set; }

        //public double T21 { get; set; }

        //public double T22 { get; set; }

        //public double T25 { get; set; }

        //public double T26 { get; set; }

        /// <summary>
        /// T3 delay syrup separation white x10
        /// </summary>
        [Property("T3")]
        public double SyrupSeparationDelay { get; set; }

        //public double T30 { get; set; }

        //public double T31 { get; set; }

        //public double T32 { get; set; }

        //public double T33 { get; set; }

        //public double T34 { get; set; }

        /// <summary>
        /// T4 delay syrup washing x10
        /// </summary>
        [Property("T4")]
        public double SyrupWashingDelay { get; set; }

        //public double T40a { get; set; }

        //public double T40b { get; set; }

        //public double T40c { get; set; }

        //public double T41a { get; set; }

        //public double T41b { get; set; }

        //public double T41c { get; set; }

        //public double T42 { get; set; }

        //public double T43 { get; set; }

        //public double T44 { get; set; }

        //public double T45 { get; set; }

        //public double T46 { get; set; }

        //public double T47 { get; set; }

        //public double T48 { get; set; }

        /// <summary>
        /// T5 duration syrup washing x10
        /// </summary>
        [Property("T5")]
        public double SyrupWashingDuration { get; set; }

        //public double T50 { get; set; }

        //public double T51 { get; set; }

        //public double T52 { get; set; }

        //public double T53 { get; set; }

        /// <summary>
        /// T6a delay water washing 1 x10
        /// </summary>
        [Property("T6a")]
        public double WaterWashingDelay { get; set; }

        //public double T6b { get; set; }

        //public double T6c { get; set; }

        //public double T7a { get; set; }

        //public double T7b { get; set; }

        //public double T7c { get; set; }

        //public double T8 { get; set; }

        //public double T9 { get; set; }

        //public DateTimeOffset time { get; set; }

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

        //public double vibration { get; set; }

        //public double warnung_error_1 { get; set; }

        //public double warnung_error_10 { get; set; }

        //public double warnung_error_11 { get; set; }

        //public double warnung_error_12 { get; set; }

        //public double warnung_error_2 { get; set; }

        //public double warnung_error_3 { get; set; }

        //public double warnung_error_4 { get; set; }

        //public double warnung_error_5 { get; set; }

        //public double warnung_error_6 { get; set; }

        //public double warnung_error_7 { get; set; }

        //public double warnung_error_8 { get; set; }

        //public double warnung_error_9 { get; set; }

        #endregion
    }
}