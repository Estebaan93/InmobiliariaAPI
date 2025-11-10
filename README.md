API con C#
Base de datos en Data/
Entity Framework
Se puede correr en CodeSpaces con el archivo compose.yml tiene las configuraciones para instalar phpMyAdmin en la maquina virtual, y guardar persistencia de datos de mysql.
Coleccion de endpoint: 
correo: esteban@correo.com | password: 123

PROPIETARIOS:
* POST: /api/Propietarios/login -- devuelve el token bearer
tipo de envio: Url encoded
Parametros: 
  correo
  password

* GET: /api/Propietarios/perfil -- obtenemos el perfil del propietario
Headers:
  Authorization: token bearer
  
* PUT: /api/Propietarios/editar  -- solo se editan los campos que se envian
Headers:
  Authorization: token bearer 
Cuerpo: json {propietario}

* PUT: /api/Propietarios/cambiarPassword -- enviar el viejo pass y el nuevo, despues de cambiar la contraseña se invalida el token anterior
tipo de envio: Url Encoded
Headers:
  Authorization: token bearer
Parametros:
  currentPassword
  newPassword


INMUEBLES:
* GET: /api/Inmuebles/obtener -- obtenemos todos los inmuebles del propietario
Headers:
  Authorization: token bearer

* POST: /api/Inmuebles/nuevo -- json {inmueble} y una imagen tipo file
tipo de envio: multi-part
Headers:
  Authorization: token bearer 
json{inmueble} + imagen{file}

* PUT: /api/Inmuebles/cambiarEstado/  -- json {idInmueble, estado}
Headers:
  Authorization: token bearer  
Cuerpo: json {idInmueble, estado}

*GET: /api/Inmueble/{id}  -- obtenemos un inmueble por su id
Headers:
  Authorization: token bearer 

* GET: /api/Inmuebles/activos -- obtenemos los que tienen contratos
Headers:
  Authorization: token bearer
  

CONTRATOS:
* GET: /api/Contratos/porInmueble/{id} -- id del inmueble con contrato activo
Headers:
  Authorization: token bearer


PAGOS:
* GET: /api/Pagos/porContrato/{id}  -- id del contrato, obtenemos los pagos
Headers:
  Authorization: token bearer
