# Stargazing VR
<p>
In this project we use XR Interaction toolkit inside Unity software to create a realtime simulation of the nightsky.
The user enters their city and using the city co ordinates and the current time, stars are populated into the scene.
</p>
<h1><u>Astronomical principles used in this project
	</u>(There is a summary below if you are too lazy to read.)</h1>


<p> There are mainly two coordinate systems in Astronomy,</p>
<ol>
  <h2><li>Horizontal coordinate system</li></h2>
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/f/f7/Azimuth-Altitude_schematic.svg/525px-Azimuth-Altitude_schematic.svg.png" alt="Horizontal coordinate system" width="500" height="500">  
	<p>The horizontal coordinate system is a celestial coordinate system that uses the observer's local horizon as the fundamental plane to define two angles: altitude and azimuth. Therefore, the horizontal coordinate system is sometimes called the az/el system, the alt/az system, or the alt-azimuth system, among others.</p>
  <h2><li>Equatorial coordinate system</li></h2>
	<img src="https://skyandtelescope.org/wp-content/uploads/RA-Dec-wiki-Tom-RuenCC-BY-SA-3.0.jpg" alt="Equatorial coordinate system" width="500" height="500">  
	<p>The Equatorial Coordinate System uses two measurements, right ascension and declination. Just as the latitude and longitude of a position on the Earth's surface define a unique location, regardless of the point of view, Right Ascension (R.A.) and Declination (Dec) of an object specify a unique position on the celestial sphere.</p>
</ol>
<h2>Summary about the coordinate systems</h2>
<p>The major difference between the two systems is that when we use the Equatorial coordinate system, the coodinates for a star is the same wherever on the earth you are, at whatever time. But in the case of Horizontal coordinate system the coordinate changes based on the observer's position and time.</p>
<h1>How the project works</h1>
<ol>
	<h2><li>Getting the data</li></h2>
	<p>
	There are three types of data that we need to simulate the nightsky in realtime.
	<br>
	<ul>
		<li>Data of stars in Equatorial coordinates.</li>
		<li>Position of the user.</li>
		<li>Current time in Universal Time Coordinated (UTC).</li>
	</ul>
	</p>
	<br>
	<p>We get our star data from the <a href="https://www.astronexus.com/hyg">HYG Star Database</a>. We sort this excel sheet by order of magnitude (magnitude is an astronomical measurement of brightness) and we take the data of the first 10,000 stars. We sort by magnitude to get the brightest stars. Keep in mind that this data is in Equatorial coordinate system. For us to simulate the nightsky in realtime, we need to convert these values from Equatorial coordinate system to Horizontal coordinate system.</p><p> We also need the user's position, so we prompt the user to enter their city name, and using a city database we get the coordinates of the city.</p><p>To get time in UTC we can use DateTime provided in the .NET framework.</p>
	<h2><li>Reading the data</li></h2>
	<p>For both the star data and the city data, we create editor scripts that convert excel sheets in csv formats to scriptable objects. This conversion only needs to happen once since the data doesn't change and hence it is done using editor scripts.</p>
	<h2><li>Converting Equatorial coordinate to Horizontal coordinate (Right Ascension and Declination to Altitude-Azimuth) </li></h2>
	<p>There is some complex math involved here and if you want to understand it deeply, you can check the website <a href="https://www.astrogreg.com/convert_ra_dec_to_alt_az.html">Greg Miller's Astronomy Programming Page</a>, but for our purposes we convert the function that is displayed on the webpage to C#. </p>
	<h2><li>And finally, generating the stars</li></h2>
	<p>So we have the Equatorial coordinates of the stars inside the scriptable object, we convert the data of each star into Horizontal coordinates. But unity uses Cartesian coordinates so we again convert the Horizontal coordinates which is a form of Spherical coordinate into Cartesian coordinate. This conversion is explained on the web page  <a href="https://www.mathworks.com/help/symbolic/transform-spherical-coordinates-and-plot.html">Mathworks.com</a>. Also note that the x,y,z axis of Cartesian coordinate in Unity is different from the x,y,z axis that is used in math. We account for this difference in the code. So now we have the position of the stars in a form which Unity can understand, and we place a simple spherical gameobject which emits light at this position. We also use the magnitude property from the database to determine how bright the star should be. </p>
</ol>
<h1>Interactions and scene setup</h1>
<p>We use XR Interaction toolkit for the UI, Unity terrain system for the basic level design, and also Shadergraph to add basic water into the scene.</p>
