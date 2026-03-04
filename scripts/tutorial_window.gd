extends ColorRect

var _current_page: int = 0

@onready var _button_previous: Button = $"Buttons/Button Previous"
@onready var _button_next: Button = $"Buttons/Button Next"
@onready var _label_pages: Label = $"Buttons/Label Pages"
@onready var _button_close: Button = $"Header/Button Close"

@onready var _pages: Array[Control] = [
	$"MarginContainer/Page 1",
	$"MarginContainer/Page 2", 
	$"MarginContainer/Page 3",
	$"MarginContainer/Page 4", 
	$"MarginContainer/Page 5", 
	$"MarginContainer/Page 6", 
	$"MarginContainer/Page 7", 
	$"MarginContainer/Page 8", 
	$"MarginContainer/Page 9", 
	$"MarginContainer/Page 10", 
	$"MarginContainer/Page 11"
]

func _ready() -> void:
	_button_close.pressed.connect(_on_close_tutorial)
	_button_previous.pressed.connect(_on_previous_page)
	_button_next.pressed.connect(_on_next_page)
	_show_page()

func _on_close_tutorial() -> void:
	hide()

func _on_previous_page() -> void:
	_current_page -= 1
	_show_page()

func _on_next_page() -> void:
	_current_page += 1
	_show_page()

func _show_page() -> void:
	if _current_page >= _pages.size() - 1:
		_current_page = _pages.size() - 1
		_button_next.disabled = true
		_button_next.mouse_default_cursor_shape = Control.CURSOR_FORBIDDEN
	elif _current_page <= 0:
		_current_page = 0
		_button_previous.disabled = true
		_button_previous.mouse_default_cursor_shape = Control.CURSOR_FORBIDDEN
	else:
		_button_next.disabled = false
		_button_next.mouse_default_cursor_shape = Control.CURSOR_POINTING_HAND
		_button_previous.disabled = false
		_button_previous.mouse_default_cursor_shape = Control.CURSOR_POINTING_HAND
	
	for page: Control in _pages:
		page.hide()
	
	_pages[_current_page].show()
	_label_pages.text = "%s / %s" % [ (_current_page + 1), _pages.size() ]
