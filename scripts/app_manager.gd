extends Node
class_name AppManager

signal on_performance_mode_changed(state: bool)

@onready var _sidebar: ColorRect = $Sidebar
@onready var _lighting_preview: ColorRect = $"Lighting Preview"
@onready var _max_fps_dropdown: OptionButton = $"Sidebar/Container/Max FPS/HBoxContainer/OptionButton"
@onready var _lang_dropdown: OptionButton = $Sidebar/Container/Language/HBoxContainer/OptionButton
@onready var _intensity_slider: HSlider = $"Sidebar/Container/Lighting Intensity/HBoxContainer/HSlider"
@onready var _intensity_spinbox: SpinBox = $"Sidebar/Container/Lighting Intensity/HBoxContainer/SpinBox"
@onready var _vtuber_result: TextureRect = $"SpoutViewport/Vtuber Result"
@onready var _perf_mode_dropdown: OptionButton = $"Sidebar/Container/Performance Mode/HBoxContainer/OptionButton"
@onready var _update_check_dropdown: OptionButton = $"Sidebar/Container/Update Check/HBoxContainer/OptionButton"
@onready var _save_manager: SaveManager = $"Save Manager"
@onready var _update_checker: UpdateChecker = $"Update Checker"

func _ready() -> void:
	_on_language_selected(_get_lang_index(_save_manager.get_language()))
	
	DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)
	_on_max_fps_selected(_get_max_fps_index(_save_manager.get_max_fps()))
	
	_on_changing_intensity(_save_manager.get_light_intensity())
	
	if _save_manager.get_performance_mode():
		_on_perf_mode_changed(0)
	else:
		_on_perf_mode_changed(1)
	
	if _save_manager.get_check_for_updates():
		_update_check_dropdown.select(0)
		_on_update_check_changed(0)
	else:
		_update_check_dropdown.select(1)
		_on_update_check_changed(1)
	
	_max_fps_dropdown.item_selected.connect(_on_max_fps_selected)
	_lang_dropdown.item_selected.connect(_on_language_selected)
	_intensity_slider.value_changed.connect(_on_changing_intensity)
	_intensity_slider.drag_ended.connect(_on_intensity_slider_done)
	_intensity_spinbox.value_changed.connect(_on_changing_intensity_spinbox)
	_perf_mode_dropdown.item_selected.connect(_on_perf_mode_changed)
	_update_check_dropdown.item_selected.connect(_on_update_check_changed)

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("toggle_sidebar"):
		_sidebar.visible = not _sidebar.visible
		_lighting_preview.visible = _sidebar.visible

func _on_max_fps_selected(index: int) -> void:
	var new_fps: int = _get_max_fps(index)
	print("Changed max FPS to %s" % new_fps)
	Engine.max_fps = new_fps
	_max_fps_dropdown.select(index)
	_save_manager.set_max_fps(new_fps)
	_save_manager.save_config()

func _get_max_fps(index: int) -> int:
	match index:
		0: return 30
		1: return 45
		2: return 50
		3: return 60
		4: return 90
		5: return 120
		_: return 60

func _get_max_fps_index(fps: int) -> int:
	match fps:
		30: return 0
		45: return 1
		50: return 2
		60: return 3
		90: return 4
		120: return 5
		_: return 3

func _on_language_selected(index: int) -> void:
	var key: String = _get_lang(index)
	
	TranslationServer.set_locale(_get_lang(index))
	print("Changed language to %s" % key)
	_lang_dropdown.select(index)
	_save_manager.set_language(key)
	_save_manager.save_config()

func _get_lang(index: int) -> String:
	match index:
		0: return "en"
		1: return "fr"
		_: return "en"

func _get_lang_index(lang: String) -> int:
	match lang:
		"en": return 0
		"fr": return 1
		_: return 0

func _on_changing_intensity(value: float) -> void:
	_intensity_slider.value = value
	_intensity_spinbox.value = value
	_vtuber_result.material.set("shader_parameter/light_strength", value)

func _on_changing_intensity_spinbox(value: float) -> void:
	print("Changed light intensity to %s" % value)
	_on_changing_intensity(value)
	_save_manager.set_light_intensity(value)
	_save_manager.save_config()

func _on_intensity_slider_done(value_changed: bool) -> void:
	if value_changed:
		print("Changed light intensity to %s" % _intensity_slider.value)
		_save_manager.set_light_intensity(_intensity_slider.value)
		_save_manager.save_config()

func _on_perf_mode_changed(index: int) -> void:
	_perf_mode_dropdown.select(index)
	if index == 0:
		print("Changed performance mode to true")
		OS.low_processor_usage_mode = true
		_save_manager.set_performance_mode(true)
		on_performance_mode_changed.emit(true)
	else:
		print("Changed performance mode to false")
		OS.low_processor_usage_mode = false
		on_performance_mode_changed.emit(false)
		_save_manager.set_performance_mode(false)
	_save_manager.save_config()

func _on_update_check_changed(index: int) -> void:
	if index == 0:
		_update_checker.check_for_updates()
		_save_manager.set_check_for_updates(true)
	else:
		_save_manager.set_check_for_updates(false)
	
	_save_manager.save_config()
