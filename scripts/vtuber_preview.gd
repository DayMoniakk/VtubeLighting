extends Control

@onready var _app: AppManager = $".."

func _ready() -> void:
	_app.on_performance_mode_changed.connect(_on_performance_mode_changed)

func _on_performance_mode_changed(state: bool) -> void:
	visible = not state
