extends Resource
class_name Config

@export var language: String = "en"
@export var max_fps: int = 60
@export var light_intensity: float = 0.0
@export var vtuber_source_name: String = ""
@export var light_source_name: String = ""
@export var performance_mode: bool = false
@export var check_for_updates: bool = true

func save(path: String) -> void:
	ResourceSaver.save(self, path)

static func load(path: String) -> Config:
	if ResourceLoader.exists(path):
		return load(path)
	
	push_warning("Config couldn't be found in \"%s\", a new one will be created" % path)
	return null
