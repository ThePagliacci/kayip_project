var map = L.map('map').setView([39.9334, 32.8597], 6);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

var marker;

map.on('click', function(e) {
 if (marker) {
  marker.setLatLng(e.latlng);
}
 else {
        marker = L.marker(e.latlng).addTo(map);
      }

    document.getElementById('Latitude').value = e.latlng.lat;
    document.getElementById('Longitude').value = e.latlng.lng;
});