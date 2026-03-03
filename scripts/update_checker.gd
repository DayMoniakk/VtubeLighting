extends Node
class_name UpdateChecker

@export var _current_version: String = "0.0.0"
@export var _version_url: String = "https://raw.githubusercontent.com/DayMoniakk/VtubeLighting/refs/heads/main/VERSION"
@export var _notif_display_time: float = 10.0

var req: HTTPRequest

@onready var _label_update: Label = $"../Sidebar/Container/Label Update"

func _ready() -> void:
	_label_update.hide()

func check_for_updates() -> void:
	req = HTTPRequest.new()
	add_child(req)
	req.request_completed.connect(_on_request_completed)
	req.request(_version_url)

func _on_request_completed(result: int, response_code: int, _headers: PackedStringArray, body: PackedByteArray) -> void:
	if result == HTTPRequest.Result.RESULT_SUCCESS and response_code == 200:
		var version_string := body.get_string_from_utf8()
		if version_string != _current_version:
			_label_update.text = tr("sb_new_update").replace("{old}", _current_version).replace("{new}", version_string)
			_label_update.show()
			print(tr(_label_update.text))
	else:
		push_error("Failed to check for update (url: %s)" % _version_url)
	
	req.queue_free()
	if _label_update.visible:
		await get_tree().create_timer(_notif_display_time).timeout
		_label_update.hide()
