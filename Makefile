build:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local build --no-cache

run:
	docker-compose -f docker-compose.yml -f docker-compose.local.yml --env-file .env.local up -d

stop:
	docker-compose stop
