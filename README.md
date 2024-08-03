# My Finances API

## Procedimientos para instalación local:

- Si es primera vez bajando el repo:

	1. En una consola (CMD, Powershell, Git Bash Console), deberás hacer un **`git clone`** + link del repositorio.
		
		- Se te van a pedir credenciales de tu cuenta que estas usando en Azure Devops, asegurarse de recordarlas/tenerlas guardadas en algun documento de texto.

	2. Deberás crear las migraciones, para esto hacer lo siguiente:
		
		- Primero que todo asegurarse que el Connection String esta previamente configurado en el **`appsettings.json`** (No aparecerá en los ultimos cambios porque este no se "commitea")
			* Ejemplo:
				"ConnectionStrings": {
					 "key-db": "Data Source=.;Initial Catalog={nombre de la base de datos};Integrated Security=True"
  				}

		- En la pestaña Tools/Herramientas abrir la NuGet Package Manager / Administrador de Paquetes NuGet.

		- Una vez abierta la consola deberas ejecutar los siguientes comandos: 
			* Add-Migration + [NOMBRE DE LA MIGRACION].
			*  Una vez completo el Build correctamente, correr el comando Update-Database. Este comando actualiza la base de datos basándose en la última migración.

- Si no es la primera vez:
	
	1. En una consola (CMD, Powershell, Git Bash Console), deberás hacer un **`git pull`** de la rama **MASTER**.
	2. Una vez traidos todos los cambios, deberás crear tu rama de desarrollo usando el siguiente comando:
		- git checkout -b + [el nombre de tu nueva rama], y listo para empezar a trabajar.
