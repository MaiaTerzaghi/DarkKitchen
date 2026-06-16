## Guía de Despliegue 
### Pre requisitos
Tener Docker Desktop instalado y corriendo, ngrok instalado y autenticado, Firebase CLI instalado y logueado (firebase login), Node.js instalado.

### Credenciales
- SQL Server (Docker): usuario SA, contraseña DarkK1tch@n

- Aplicación (admin): admin@darkkitchen.com / Passw0rd!Testing
- Firebase project: darkkitchen-msn
- URL pública: https://darkkitchen-msn.web.app
### Levantar el entorno completo
#### Paso 1: 
Abrir Docker Desktop y verificar que esté corriendo.
#### Paso 2: 
Crear la red Docker (solo la primera vez):
 	docker network create da2-network
#### Paso 3:
 Levantar SQL Server.

##### Primera vez:
 docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=DarkK1tch@n" -p 1433:1433 --name sql --network da2-network -d mcr.microsoft.com/mssql/server:2025-latest
##### Si ya existe el container:
docker start sql
#### Paso 4: 
Restaurar la base de datos (solo si no tiene datos).

##### Generar backup desde SQL Express local:
sqlcmd -S .\SQLEXPRESS -Q "BACKUP DATABASE DarkKitchen TO DISK='C:\temp\DarkKitchen.bak' WITH FORMAT"
##### Copiar al container y restaurar:
docker cp C:\temp\DarkKitchen.bak sql:/var/opt/mssql/data/DarkKitchen.bak
docker exec -it sql /opt/mssql-tools2/bin/sqlcmd -S localhost -U SA -P "DarkK1tch@n" -Q "RESTORE DATABASE DarkKitchen FROM DISK='/var/opt/mssql/data/DarkKitchen.bak' WITH MOVE 'DarkKitchen' TO '/var/opt/mssql/data/DarkKitchen.mdf', MOVE 'DarkKitchen_log' TO '/var/opt/mssql/data/DarkKitchen_log.ldf', REPLACE"
#### Paso 5: 
Buildear la imagen de la API desde la carpeta raíz del repositorio:
docker build -f DarkKitchen.WebApi/Dockerfile -t darkkitchen-api .
#### Paso 6: Correr el container de la API:
docker run -d --name api --network da2-network -p 5222:5222 -e "ConnectionStrings__DarkKitchen=Server=sql,1433;Database=DarkKitchen;User Id=SA;Password=DarkK1tch@n;TrustServerCertificate=True;Encrypt=False" darkkitchen-api
##### Si ya existe: 
docker start api
#### Paso 7: 
##### Levantar ngrok en otra terminal:
ngrok http 5222

Copiar la URL HTTPS que aparece. La URL cambia cada vez que se reinicia ngrok (plan free).
#### Paso 8: 
Actualizar la URL en Frontend/src/environments/environment.production.ts
#### Paso 9: 
Buildear y deployar el frontend desde la carpeta Frontend/:

- npm run build

- firebase deploy --only hosting
#### Paso 10:
 Verificar en https://darkkitchen-msn.web.app con las credenciales de admin.
##### Si ya estaba todo levantado de antes
Solo se necesita: docker start sql, docker start api, levantar ngrok (ngrok http 5222), actualizar environment.production.ts con la nueva URL, y rebuildar y deployar el frontend (npm run build + firebase deploy --only hosting).
#### Problemas comunes 
- CORS errors en consola: verificar que la URL en environment.production.ts coincida con la de ngrok y redesplegar.
- Credenciales inválidas al hacer login: verificar que la base se restauró correctamente y reiniciar el container con docker restart api.
-  API no conecta a SQL: verificar que ambos containers estén en la red da2-network con docker network inspect da2-network.
