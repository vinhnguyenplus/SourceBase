# start database
docker compose up

# start backend
cd Admin.NET/Admin.NET.Web.Entry
dotnet run -f net10.0

# start frontend
cd Web
pnpm install
pnpm dev