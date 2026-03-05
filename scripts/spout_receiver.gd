# Failed attempt to receive spout data without taking 25% gpu usage for each active SpoutTexture
# I don't really get this spout bindings, there seem to be an issue between the opengl and the directX/vulkan one

extends RefCounted
class_name SpoutReceiver

const GL_TEXTURE_2D := 3553

var _spout: Spout
var _texture: ImageTexture

func _init() -> void:
	_create_texture(1, 1)
	_spout = Spout.new()

func _create_texture(w: int, h: int) -> void:
	var img := Image.create(w, h, false, Image.FORMAT_RGBA8)
	_texture = ImageTexture.create_from_image(img)

	RenderingServer.texture_set_force_redraw_if_visible(
		_texture.get_rid(),
		true
	)
	await RenderingServer.frame_post_draw
	
func set_sender_name(sender_name: String) -> void:
	_spout.release_receiver()
	_spout.set_receiver_name(sender_name)

func get_texture() -> Texture:
	return _texture

func poll() -> void:
	if _spout.get_sender_name() == "":
		return

	var tex_id := RenderingServer.texture_get_native_handle(_texture.get_rid())

	# Attempt receive every frame (this establishes connection)
	_spout.receive_texture(tex_id, GL_TEXTURE_2D, false, 0)

	var w := _spout.get_sender_width()
	var h := _spout.get_sender_height()

	if w > 0 and h > 0:
		if _texture.get_width() != w or _texture.get_height() != h:
			print("[Spout Receiver] image dimensions changed: %sx%s" % [w, h])
			_create_texture(w, h)
