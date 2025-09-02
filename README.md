To start the back end server:

cd server/HnStories.Api
dotnet restore
dotnet run

Note the port that the API starts on. It is http://localhost:5000 on my machine. If that's not the port that comes up, edit environment.ts and environment.prod.ts with the correct port.

To run the back end tests:

cd ../HnStories.Tests
dotnet test

To start the front end client:

cd client
npm install
npm start

To run the front end tests:

npm test
