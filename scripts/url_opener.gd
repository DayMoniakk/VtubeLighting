extends Button

@export var _url: String

func _ready() -> void:
	pressed.connect(_on_clicked)

func _on_clicked() -> void:
	OS.shell_open(_url)
