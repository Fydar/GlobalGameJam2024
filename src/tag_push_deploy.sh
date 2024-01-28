set -x

# Login

# aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 222779217717.dkr.ecr.us-east-1.amazonaws.com

# Build

docker build . -t ggj24instance -f GlobalGameJam2024.WebApp/Dockerfile

# Tag

docker tag ggj24instance:latest 222779217717.dkr.ecr.us-east-1.amazonaws.com/ggj24instance:latest

# Deploy

docker push 222779217717.dkr.ecr.us-east-1.amazonaws.com/ggj24instance:latest
