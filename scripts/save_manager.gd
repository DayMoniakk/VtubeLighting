extends Node
class_name SaveManager

@export var _config_path: String = "user://config.tres"

var _config: Config

func _enter_tree() -> void:
	_config = Config.load(_config_path)
	if _config == null:
		_config = Config.new()
		save_config()

func get_language() -> String: return _config.language
func get_max_fps() -> int: return _config.max_fps
func get_light_intensity() -> float: return _config.light_intensity
func get_vtuber_source_name() -> String: return _config.vtuber_source_name
func get_light_source_name() -> String: return _config.light_source_name
func get_performance_mode() -> bool: return _config.performance_mode
func get_check_for_updates() -> bool: return _config.check_for_updates

func set_language(lang: String) -> void: _config.language = lang
func set_max_fps(fps: int) -> void: _config.max_fps = fps
func set_light_intensity(value: float) -> void: _config.light_intensity = value
func set_vtuber_source_name(source_name: String) -> void: _config.vtuber_source_name = source_name
func set_light_source_name(source_name: String) -> void: _config.light_source_name = source_name
func set_performance_mode(value: bool) -> void: _config.performance_mode = value
func set_check_for_updates(value: bool) -> void: _config.check_for_updates = value

func save_config() -> void:
	_config.save(_config_path)
