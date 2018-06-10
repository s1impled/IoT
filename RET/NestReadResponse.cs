using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RET
{
    public sealed class NestReadResponse
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "data")]
        public NestData Data { get; set; }

    }

    public sealed class NestData
    {
        [JsonProperty(PropertyName = "devices")]
        public Devices Devices { get; set; }

        [JsonProperty(PropertyName = "structures")]
        public IDictionary<string, Structure> Structures { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }

    public sealed class Devices
    {
        [JsonProperty(PropertyName = "thermostats")]
        public IDictionary<string, Thermostat> Thermostats { get; set; }

        [JsonProperty(PropertyName = "smoke_co_alarms")]
        public IDictionary<string, SmokeCoAlarm> SmokeCoAlarms { get; set; }

        [JsonProperty(PropertyName = "cameras")]
        public IDictionary<string, Camera> Cameras { get; set; }

        [JsonProperty(PropertyName = "$company")]
        public IDictionary<string, Product> Company { get; set; }

    }

    public sealed class Product
    {
        [JsonProperty(PropertyName = "$product_type")]
        public IDictionary<string, ProductType> ProductTypes { get; set; }
    }

    public sealed class ProductType
    {
        public Identification identification { get; set; }
        public Location location { get; set; }
        public Software software { get; set; }
        public IDictionary<string, ResourceUse> resource_use { get; set; }
    }

    public sealed class Identification
    {
        public string device_id { get; set; }
        public string serial_number { get; set; }
    }

    public sealed class Location
    {
        public string structure_id { get; set; }
        public string where_id { get; set; }
    }

    public sealed class Software
    {
        public string version { get; set; }
    }

    public sealed class ResourceUse
    {
        public string value { get; set; }
        public DateTimeOffset measurement_reset_time { get; set; }
        public DateTimeOffset measurement_time { get; set; }
    }

    public sealed class SmokeCoAlarm
    {
        public string device_id { get; set; }
        public string locale { get; set; }
        public string software_version { get; set; }
        public string structure_id { get; set; }
        public string name { get; set; }
        public string name_long { get; set; }
        public DateTimeOffset last_connection { get; set; }
        public bool is_online { get; set; }
        public string battery_health { get; set; }
        public string co_alarm_state { get; set; }
        public string smoke_alarm_state { get; set; }
        public bool is_manual_test_active { get; set; }
        public DateTimeOffset last_manual_test_time { get; set; }
        public string ui_color_state { get; set; }
        public string where_id { get; set; }
        public string where_name { get; set; }
    }

    public sealed class Thermostat
    {
        public int humidity { get; set; }
        public string locale { get; set; }
        public string temperature_scale { get; set; }
        public bool is_using_emergency_heat { get; set; }
        public bool has_fan { get; set; }
        public string software_version { get; set; }
        public bool has_leaf { get; set; }
        public string where_id { get; set; }
        public string device_id { get; set; }
        public string name { get; set; }
        public bool can_heat { get; set; }
        public bool can_cool { get; set; }
        public double target_temperature_c { get; set; }
        public double target_temperature_f { get; set; }
        public double target_temperature_high_c { get; set; }
        public double target_temperature_high_f { get; set; }
        public double target_temperature_low_c { get; set; }
        public double target_temperature_low_f { get; set; }
        public double ambient_temperature_c { get; set; }
        public double ambient_temperature_f { get; set; }
        public double away_temperature_high_c { get; set; }
        public double away_temperature_high_f { get; set; }
        public double away_temperature_low_c { get; set; }
        public double away_temperature_low_f { get; set; }
        public double eco_temperature_high_c { get; set; }
        public double eco_temperature_high_f { get; set; }
        public double eco_temperature_low_c { get; set; }
        public double eco_temperature_low_f { get; set; }
        public bool is_locked { get; set; }
        public double locked_temp_min_c { get; set; }
        public double locked_temp_min_f { get; set; }
        public double locked_temp_max_c { get; set; }
        public double locked_temp_max_f { get; set; }
        public bool sunlight_correction_active { get; set; }
        public bool sunlight_correction_enabled { get; set; }
        public string structure_id { get; set; }
        public bool fan_timer_active { get; set; }
        public DateTimeOffset fan_timer_timeout { get; set; }
        public int fan_timer_duration { get; set; }
        public string previous_hvac_mode { get; set; }
        public string hvac_mode { get; set; }
        public string time_to_target { get; set; }
        public string time_to_target_training { get; set; }
        public string where_name { get; set; }
        public string label { get; set; }
        public string name_long { get; set; }
        public bool is_online { get; set; }
        public DateTimeOffset last_connection { get; set; }
        public string hvac_state { get; set; }
    }

    public sealed class Camera
    {
        public string device_id { get; set; }
        public string software_version { get; set; }
        public string structure_id { get; set; }
        public string where_id { get; set; }
        public string where_name { get; set; }
        public string name { get; set; }
        public string name_long { get; set; }
        public bool is_online { get; set; }
        public bool is_streaming { get; set; }
        public bool is_audio_input_enabled { get; set; }
        public DateTimeOffset last_is_online_change { get; set; }
        public bool is_video_history_enabled { get; set; }
        public string web_url { get; set; }
        public string app_url { get; set; }
        public bool is_public_share_enabled { get; set; }
        public IList<ActivityZone> activity_zones { get; set; }
        public string public_share_url { get; set; }
        public string snapshot_url { get; set; }
        public LastEvent last_event { get; set; }
    }

    public sealed class LastEvent
    {
        public bool has_sound { get; set; }
        public bool has_motion { get; set; }
        public bool has_person { get; set; }
        public DateTimeOffset start_time { get; set; }
        public DateTimeOffset end_time { get; set; }
        public DateTimeOffset urls_expire_time { get; set; }
        public string web_url { get; set; }
        public string app_url { get; set; }
        public string image_url { get; set; }
        public string animated_image_url { get; set; }
        public IList<string> activity_zone_ids { get; set; }
    }

    public sealed class ActivityZone
    {
        public string name { get; set; }
        public string id { get; set; }
    }


    public sealed class Structure
    {
        public string name { get; set; }
        public string country_code { get; set; }
        public string postal_code { get; set; }
        public string time_zone { get; set; }
        public string away { get; set; }
        public IList<string> thermostats { get; set; }
        public IList<string> smoke_co_alarms { get; set; }
        public IList<string> cameras { get; set; }
        public string structure_id { get; set; }
        public bool rhr_enrollment { get; set; }
        public IDictionary<string, Where> wheres { get; set; }
        public DateTimeOffset peak_period_start_time { get; set; }
        public DateTimeOffset peak_period_end_time { get; set; }
        public DateTimeOffset eta_begin { get; set; }
        public string co_alarm_state { get; set; }
        public string smoke_alarm_state { get; set; }
        public Eta eta { get; set; }

    }

    public sealed class Eta
    {
        public string trip_id { get; set; }
        public DateTimeOffset estimated_arrival_window_begin { get; set; }
        public DateTimeOffset estimated_arrival_window_end { get; set; }
    }

    public sealed class Where
    {
        public string where_id { get; set; }
        public string name { get; set; }
    }

    public sealed class Metadata
    {
        public string access_token { get; set; }
        public int client_version { get; set; }
    }

}
