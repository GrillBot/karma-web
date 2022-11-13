git pull --rebase;
git push;
docker build -t ghcr.io/grillbot/karma-web .
docker push ghcr.io/grillbot/karma-web
