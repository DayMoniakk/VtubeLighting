extends Node

enum SourceType { None, Vtuber, Light }

@export var _source_type: SourceType
@export var _preferred_source_names: Array[String]
@export var _source_dropdown: OptionButton
@export var _refresh_button: Button

@onready var _spout_manager: SpoutManager = $".."
@onready var _save_manager: SaveManager = $"../../Save Manager"

func _ready() -> void:
	_refresh_button.pressed.connect(_spout_manager._refresh_senders_list)
	_source_dropdown.item_selected.connect(_on_source_selected)
	_spout_manager.on_source_refreshed.connect(_refresh_options)
	
	if _source_type == SourceType.None:
		return
	
	await get_tree().process_frame
	_refresh_options()
	
	var loaded_source_name: String = ""
	match _source_type:
		SourceType.Vtuber: loaded_source_name = _save_manager.get_vtuber_source_name()
		SourceType.Light: loaded_source_name = _save_manager.get_light_source_name()
		_: push_warning("Source type not selected")
		
	if loaded_source_name == "":
		return
	if not _spout_manager._available_sources.has(loaded_source_name):
		return
	
	match _source_type:
		SourceType.Vtuber: 
			_spout_manager.set_vtuber_sender(loaded_source_name)
			print("Loaded spout source \"%s\" for the vtuber source" % loaded_source_name)
		SourceType.Light: 
			_spout_manager.set_light_sender(loaded_source_name)
			print("Loaded spout source \"%s\" for the light source" % loaded_source_name)
		_:
			push_warning("Source type not selected")
	
	for i: int in range(_source_dropdown.item_count):
		if _source_dropdown.get_item_text(i) == loaded_source_name:
			_source_dropdown.select(i)
			break

func _refresh_options() -> void:
	_source_dropdown.clear()
	var available: Array[String] = _spout_manager.get_available_sources()
	for source: String in available:
		if source != "VtubeLighting":
			_source_dropdown.add_item(source)
	
	var new_sender: String = ""
	
	if available.size() == 0:
		_source_dropdown.select(-1)
	elif _preferred_source_names.size() > 0:
		for i: int in range(available.size()):
			if available[i] == "VtubeLighting":
				continue
				
			if _preferred_source_names.has(available[i]):
				_source_dropdown.select(i)
				new_sender = available[i]
				print("Automatically selected \"%s\" as favorite source" % new_sender)
				break
				
	match _source_type:
		SourceType.Vtuber:
			print("Automatically assigned spout source \"%s\" for the vtuber source" % new_sender)
			_spout_manager.set_vtuber_sender(new_sender)
		SourceType.Light:
			print("Automatically assigned spout source \"%s\" for the light source" % new_sender)
			_spout_manager.set_light_sender(new_sender)
		_:
			push_warning("Source type not selected")
	
	if new_sender == "":
		_source_dropdown.select(-1)
	else:
		for i: int in range(_source_dropdown.item_count):
			if _source_dropdown.get_item_text(i) == new_sender:
				_source_dropdown.select(i)
				break

func _on_source_selected(index: int) -> void:
	match _source_type:
		SourceType.Vtuber:
			_spout_manager.set_vtuber_sender_by_id(index)
		SourceType.Light:
			_spout_manager.set_light_sender_by_id(index)
		SourceType.None:
			push_warning("Source type is currently set to none")
