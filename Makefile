build:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local build --no-cache

run-api:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local up -d

run-web-client:
	make -C ./src/client/diagraph run-web-client

run: run-api run-web-client

stop:
	docker-compose stop

test: test-api

test-api:
	dotnet build src/api && dotnet test src/api --no-build -l="console;verbosity=normal"
