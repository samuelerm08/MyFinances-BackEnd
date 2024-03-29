# My Finances API

## Procedimientos para instalaci�n local:

- Si es primera vez bajando el repo:

	1. En una consola (CMD, Powershell, Git Bash Console), deber�s hacer un **`git clone`** + link del repositorio.
		
		- Se te van a pedir credenciales de tu cuenta que estas usando en Azure Devops, asegurarse de recordarlas/tenerlas guardadas en algun documento de texto.

	2. Deber�s crear las migraciones, para esto hacer lo siguiente:
		
		- Primero que todo asegurarse que el Connection String esta previamente configurado en el **`appsettings.json`** (No aparecer� en los ultimos cambios porque este no se "commitea")
			* Ejemplo:
				"ConnectionStrings": {
					 "key-db": "Data Source=.;Initial Catalog={nombre de la base de datos};Integrated Security=True"
  				}

		- En la pesta�a Tools/Herramientas abrir la NuGet Package Manager / Administrador de Paquetes NuGet.

		- Una vez abierta la consola deberas ejecutar los siguientes comandos: 
			* Add-Migration + [NOMBRE DE LA MIGRACION].
			*  Una vez completo el Build correctamente, correr el comando Update-Database. Este comando actualiza la base de datos bas�ndose en la �ltima migraci�n.

- Si no es la primera vez:
	
	1. En una consola (CMD, Powershell, Git Bash Console), deber�s hacer un **`git pull`** de la rama **MASTER**.
	2. Una vez traidos todos los cambios, deber�s crear tu rama de desarrollo usando el siguiente comando:
		- git checkout -b + [el nombre de tu nueva rama], y listo para empezar a trabajar.

## Procedimientos para enviar cambios

- **Ante todo recordar no hacer commit del **`appsettings.json`** y otros archivos como la carpeta Migrations...**

- Una vez completado un cambio (incluyendo commit), basta con ejecutar en la consola de linea de comandos que est�s usando en el momento **`git push -u --all`**

- Luego de enviar cambios, en Azure Devops podr�s generar un Pull Request de tu rama para hacer el **merge** a **master**.

- Una vez creado el PR y sea finalmente aprobando, podr�s completarlo y tus cambios quedar�n impactados en **master**.