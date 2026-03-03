extends Control

@onready var _app: AppManager = $".."
@onready var _default_tooltip: String = tooltip_text

func _ready() -> void:
	_app.on_performance_mode_changed.connect(_on_performance_mode_changed)

func _on_performance_mode_changed(state: bool) -> void:
	modulate.a = 0.0 if state else 1.0
	tooltip_text = "" if state else _default_tooltip
	mouse_default_cursor_shape = Control.CURSOR_ARROW if state  else Control.CURSOR_HELP
