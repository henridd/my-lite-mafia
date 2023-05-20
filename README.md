# My Lite Mafia
Simple WPF application which uses Tile38 for Geofencing. It contains a small canvas that simulates a map, and allows creation of establishments and rivals. When a rival is created of moved inside the area of an establishment, a geofence notification is raised.
For a better explanation, access https://intodot.net/creating-geofences-in-net-with-tile38/

# Running the application
The only requiriment for it is to have Tile38 running, which can be done via Docker.
```
docker run -p 9851:9851 tile38/tile38
```
