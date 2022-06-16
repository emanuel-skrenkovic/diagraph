build:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local build --no-cache

run-api:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local up -d

run-client:
	npm run --prefix src/client/diagraph-dashboard start

run: run-api run-client

stop:
	docker-compose stop
