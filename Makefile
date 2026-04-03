ROOT_DIR:=$(shell dirname $(realpath $(lastword $(MAKEFILE_LIST))))
BACKEND_DIR:=$(ROOT_DIR)/backend
FRONTEND_DIR:=$(ROOT_DIR)/frontend
DOCKER_DIR:=$(ROOT_DIR)/docker

.PHONY: help build run clean install migrate seed test docker-up docker-down

help:
	@echo "EcommerceSaaS Makefile Commands:"
	@echo "  make install      - Install all dependencies"
	@echo "  make build        - Build backend and frontend"
	@echo "  make run          - Run backend and frontend (separate terminals)"
	@echo "  make docker-up    - Start services with Docker Compose"
	@echo "  make docker-down  - Stop Docker Compose services"
	@echo "  make migrate      - Run database migrations"
	@echo "  make seed         - Seed database with sample data"
	@echo "  make test         - Run tests"
	@echo "  make clean        - Clean build artifacts"

install:
	@echo "Installing backend dependencies..."
	cd $(BACKEND_DIR) && dotnet restore
	@echo "Installing frontend dependencies..."
	cd $(FRONTEND_DIR) && npm install
	@echo "✓ Dependencies installed"

build:
	@echo "Building backend..."
	cd $(BACKEND_DIR) && dotnet build
	@echo "Building frontend..."
	cd $(FRONTEND_DIR) && npm run build
	@echo "✓ Build complete"

run-backend:
	cd $(BACKEND_DIR) && dotnet run --project src/EcommerceSaaS.API/EcommerceSaaS.API.csproj

run-frontend:
	cd $(FRONTEND_DIR) && npm run dev

run: install
	@echo "Starting backend and frontend..."
	@echo "Backend: http://localhost:5000"
	@echo "Frontend: http://localhost:3000"
	@echo "Run in separate terminals:"
	@echo "  Terminal 1: make run-backend"
	@echo "  Terminal 2: make run-frontend"

docker-up:
	@echo "Starting Docker services..."
	cd $(DOCKER_DIR) && docker-compose up -d
	@echo "✓ Services started"
	@echo "Frontend: http://localhost:3000"
	@echo "API: http://localhost:5000"

docker-down:
	@echo "Stopping Docker services..."
	cd $(DOCKER_DIR) && docker-compose down
	@echo "✓ Services stopped"

docker-logs:
	cd $(DOCKER_DIR) && docker-compose logs -f

migrate:
	@echo "Running database migrations..."
	cd $(BACKEND_DIR)/src/EcommerceSaaS.API && \
	dotnet ef database update --project ../EcommerceSaaS.Infrastructure
	@echo "✓ Migrations complete"

seed:
	@echo "Seeding database..."
	@echo "TODO: Implement seed data"

test:
	@echo "Running tests..."
	cd $(BACKEND_DIR) && dotnet test

clean:
	@echo "Cleaning up..."
	cd $(BACKEND_DIR) && dotnet clean
	cd $(FRONTEND_DIR) && rm -rf dist node_modules
	@echo "✓ Clean complete"

lint:
	@echo "Linting code..."
	cd $(FRONTEND_DIR) && npm run lint
	@echo "✓ Linting complete"
