extends Node
class_name SpoutManager

signal on_source_refreshed

var _spout: Spout
var _received_background: SpoutTexture
var _received_vtuber: SpoutTexture
var _available_sources: Array[String]

@onready var _light_bg: TextureRect = $"../Lighting SubViewport/Pixelated BG"
@onready var _vtuber_result: TextureRect = $"../SpoutViewport/Vtuber Result"

func _ready() -> void:
	_spout = Spout.new()
	
	_received_background = SpoutTexture.new()
	_light_bg.texture = _received_background
	_received_background.sender_name = "STUPID_FIX_FOR_STUPID_BUG"
	
	_received_vtuber = SpoutTexture.new()
	_vtuber_result.texture = _received_vtuber
	_received_vtuber.sender_name = "STUPID_FIX_FOR_STUPID_BUG2"
	
	_refresh_senders_list()

func set_vtuber_sender(source_name: String) -> void:
	if _available_sources.has(source_name):
		_received_vtuber.sender_name = source_name

func set_light_sender(source_name: String) -> void:
	if _available_sources.has(source_name):
		_received_background.sender_name = source_name

func set_vtuber_sender_by_id(source_index: int) -> void:
	if _available_sources.size() > source_index:
		_received_vtuber.sender_name = _available_sources[source_index]

func set_light_sender_by_id(source_index: int) -> void:
	if _available_sources.size() > source_index:
		_received_background.sender_name = _available_sources[source_index]

func get_available_sources() -> Array[String]:
	return _available_sources

func _refresh_senders_list() -> void:
	var available: Array[String]
	for i: int in range(_spout.get_sender_count()):
		available.push_back(_spout.get_sender(i))
	_available_sources = available
	on_source_refreshed.emit()
