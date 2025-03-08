FROM node:18
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN ls -l /app && ls -l /app/src
CMD ["npx", "ts-node", "src/main.ts"]
