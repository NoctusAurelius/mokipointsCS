<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="mokipointsCS.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MOKI POINTS - Chore & Point System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="icon" type="image/x-icon" href="/favicon/favicon.ico" />
    <link rel="icon" type="image/png" sizes="16x16" href="/favicon/favicon-16x16.png" />
    <link rel="icon" type="image/png" sizes="32x32" href="/favicon/favicon-32x32.png" />
    <link rel="apple-touch-icon" sizes="180x180" href="/favicon/apple-touch-icon.png" />
    <link rel="manifest" href="/favicon/site.webmanifest" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: Arial, sans-serif;
            background-color: white;
            color: #333;
            overflow-x: hidden;
        }
        
        /* Splash Screen */
        .splash-screen {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: white;
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 9999;
            animation: fadeOut 1s ease-in-out 2s forwards;
        }
        
        .splash-branding {
            font-size: 72px;
            font-weight: bold;
            letter-spacing: 8px;
            animation: breathe 2s ease-in-out infinite;
        }
        
        .splash-branding .moki {
            color: #0066CC; /* Blue */
            display: inline-block;
        }
        
        .splash-branding .points {
            color: #FF6600; /* Orange */
            display: inline-block;
        }
        
        @keyframes breathe {
            0%, 100% {
                transform: scale(1);
                opacity: 1;
            }
            50% {
                transform: scale(1.1);
                opacity: 0.8;
            }
        }
        
        @keyframes fadeOut {
            to {
                opacity: 0;
                visibility: hidden;
            }
        }
        
        /* Main Content - Hidden initially */
        .main-content {
            opacity: 0;
            animation: fadeIn 1s ease-in-out 2.5s forwards;
        }
        
        @keyframes fadeIn {
            to {
                opacity: 1;
            }
        }
        
        /* Navigation Bar */
        .navbar {
            position: fixed;
            top: 0;
            width: 100%;
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            z-index: 1000;
            padding: 15px 0;
        }
        
        .nav-container {
            max-width: 1200px;
            margin: 0 auto;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 30px;
        }
        
        .brand {
            font-size: 24px;
            font-weight: bold;
            letter-spacing: 2px;
        }
        
        .brand .moki {
            color: #0066CC; /* Blue */
        }
        
        .brand .points {
            color: #FF6600; /* Orange */
        }
        
        .nav-links {
            display: flex;
            gap: 30px;
            list-style: none;
        }
        
        .nav-links a {
            text-decoration: none;
            color: #333;
            font-weight: 500;
            transition: color 0.3s;
            cursor: pointer;
        }
        
        .nav-links a:hover {
            color: #0066CC;
        }
        
        /* Sections */
        .section {
            padding: 60px 30px;
            max-width: 1200px;
            margin: 0 auto;
        }
        
        .section-title {
            font-size: 32px;
            margin-bottom: 30px;
            text-align: center;
        }
        
        /* Hero Section */
        #hero {
            min-height: 80vh;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            padding: 100px 30px 60px;
            background-image: url('Images/Landing/bacgkround1.jpg');
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            position: relative;
        }
        
        #hero::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(255, 255, 255, 0.7);
            z-index: 0;
        }
        
        #hero > * {
            position: relative;
            z-index: 1;
        }
        
        .hero-branding {
            font-size: 48px;
            font-weight: bold;
            letter-spacing: 4px;
            margin-bottom: 20px;
        }
        
        .hero-branding .moki {
            color: #0066CC; /* Blue */
        }
        
        .hero-branding .points {
            color: #FF6600; /* Orange */
        }
        
        .hero-subtitle {
            font-size: 18px;
            color: #666;
            margin-bottom: 30px;
        }
        
        .hero-buttons {
            display: flex;
            gap: 20px;
            margin-top: 30px;
        }
        
        .btn {
            padding: 12px 30px;
            font-size: 16px;
            font-weight: bold;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            transition: transform 0.3s, box-shadow 0.3s;
        }
        
        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .btn-primary {
            background-color: #0066CC; /* Blue */
            color: white;
        }
        
        .btn-secondary {
            background-color: #FF6600; /* Orange */
            color: white;
        }
        
        /* About Section */
        #about {
            background-color: white;
        }
        
        .about-content {
            font-size: 18px;
            line-height: 1.8;
            text-align: center;
            max-width: 800px;
            margin: 0 auto;
        }
        
        .about-text {
            margin-bottom: 40px;
        }
        
        .video-container {
            position: relative;
            padding-bottom: 56.25%; /* 16:9 aspect ratio */
            height: 0;
            overflow: hidden;
            max-width: 800px;
            margin: 0 auto;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            border-radius: 8px;
        }
        
        .video-container iframe {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            border-radius: 8px;
        }
        
        /* Updates Section */
        #updates {
            background-color: #FFF9E6; /* Light yellow background */
        }
        
        .updates-content {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 30px;
            margin-top: 40px;
        }
        
        .update-card {
            background-color: white;
            padding: 0;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            border-left: 4px solid #FFB6C1; /* Pink accent */
            overflow: hidden;
            cursor: pointer;
            transition: transform 0.3s, box-shadow 0.3s;
        }
        
        .update-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }
        
        .update-image {
            width: 100%;
            height: 200px;
            object-fit: cover;
            display: block;
        }
        
        .update-content {
            padding: 20px;
        }
        
        .update-date {
            color: #999;
            font-size: 12px;
            margin-bottom: 8px;
        }
        
        .update-title {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 10px;
            color: #0066CC;
        }
        
        .update-content p {
            font-size: 14px;
            line-height: 1.6;
            color: #666;
        }
        
        /* Modal Styles */
        .update-modal {
            display: none;
            position: fixed;
            z-index: 10000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.8);
            animation: fadeIn 0.3s;
        }
        
        .update-modal.active {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }
        
        .modal-content {
            background-color: white;
            margin: auto;
            padding: 0;
            border-radius: 10px;
            max-width: 900px;
            width: 100%;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
            animation: slideDown 0.3s;
        }
        
        @keyframes slideDown {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }
        
        .modal-header {
            position: relative;
            width: 100%;
        }
        
        .modal-image {
            width: 100%;
            max-height: 400px;
            object-fit: cover;
            display: block;
        }
        
        .modal-close {
            position: absolute;
            top: 15px;
            right: 15px;
            color: white;
            font-size: 32px;
            font-weight: bold;
            cursor: pointer;
            background-color: rgba(0,0,0,0.5);
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: background-color 0.3s;
        }
        
        .modal-close:hover {
            background-color: rgba(0,0,0,0.8);
        }
        
        .modal-body {
            padding: 40px;
        }
        
        .modal-date {
            color: #999;
            font-size: 14px;
            margin-bottom: 15px;
        }
        
        .modal-title {
            font-size: 28px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #0066CC;
        }
        
        .modal-text {
            font-size: 16px;
            line-height: 1.8;
            color: #333;
            margin-bottom: 20px;
        }
        
        .modal-text p {
            margin-bottom: 15px;
        }
        
        /* Contact Section */
        #contact {
            background-color: white;
        }
        
        .contact-content {
            max-width: 1000px;
            margin: 0 auto;
        }
        
        .contact-info {
            font-size: 18px;
            line-height: 2;
            margin-top: 30px;
            text-align: center;
        }
        
        .contact-details {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            margin-top: 40px;
            align-items: start;
        }
        
        .contact-info-section {
            text-align: left;
        }
        
        .contact-info-section h3 {
            color: #0066CC;
            margin-bottom: 20px;
            font-size: 24px;
        }
        
        .contact-info-section p {
            margin-bottom: 15px;
            font-size: 16px;
        }
        
        .contact-info-section a {
            color: #0066CC;
            text-decoration: none;
        }
        
        .contact-info-section a:hover {
            text-decoration: underline;
        }
        
        .map-container {
            width: 100%;
            height: 400px;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .map-container iframe {
            width: 100%;
            height: 100%;
            border: 0;
        }
        
        @media (max-width: 768px) {
            .contact-details {
                grid-template-columns: 1fr;
            }
        }
        
        /* Footer */
        #footer {
            background-color: #333;
            color: white;
            text-align: center;
            padding: 40px 30px;
        }
        
        .footer-content {
            max-width: 1200px;
            margin: 0 auto;
        }
        
        .footer-brand {
            font-size: 24px;
            margin-bottom: 20px;
        }
        
        .footer-brand .moki {
            color: #0066CC;
        }
        
        .footer-brand .points {
            color: #FF6600;
        }
        
        .footer-text {
            color: #ccc;
        }
    </style>
    <script>
        // Smooth scroll to sections
        document.addEventListener('DOMContentLoaded', function() {
            var navLinks = document.querySelectorAll('.nav-links a');
            
            navLinks.forEach(function(link) {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    var targetId = this.getAttribute('href').substring(1);
                    var targetSection = document.getElementById(targetId);
                    
                    if (targetSection) {
                        targetSection.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                });
            });
        });
        
        // Update Modal Functions
        function openUpdateModal(modalNumber) {
            var modal = document.getElementById('updateModal' + modalNumber);
            if (modal) {
                modal.classList.add('active');
                document.body.style.overflow = 'hidden'; // Prevent background scrolling
            }
        }
        
        function closeUpdateModal(modalNumber) {
            var modal = document.getElementById('updateModal' + modalNumber);
            if (modal) {
                modal.classList.remove('active');
                document.body.style.overflow = 'auto'; // Restore scrolling
            }
        }
        
        function closeUpdateModalOnBackdrop(event, modalNumber) {
            // Close modal if clicking on the backdrop (not the content)
            if (event.target.classList.contains('update-modal')) {
                closeUpdateModal(modalNumber);
            }
        }
        
        // Close modal with Escape key
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                for (var i = 1; i <= 3; i++) {
                    var modal = document.getElementById('updateModal' + i);
                    if (modal && modal.classList.contains('active')) {
                        closeUpdateModal(i);
                    }
                }
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Splash Screen -->
        <div class="splash-screen">
            <div class="splash-branding">
                <span class="moki">MOKI</span><span class="points"> POINTS</span>
            </div>
        </div>

        <!-- Main Content -->
        <div class="main-content">
        <!-- Navigation Bar -->
        <nav class="navbar">
            <div class="nav-container">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <ul class="nav-links">
                    <li><a href="#hero">Home</a></li>
                    <li><a href="#about">About</a></li>
                    <li><a href="#updates">Updates</a></li>
                    <li><a href="#contact">Contact</a></li>
                </ul>
            </div>
        </nav>

        <!-- Hero Section -->
        <section id="hero" class="section">
            <div class="hero-branding">
                <span class="moki">MOKI</span><span class="points"> POINTS</span>
            </div>
            <p class="hero-subtitle">Your Family Chore & Point System</p>
            <div class="hero-buttons">
                <a href="Register.aspx" class="btn btn-primary">Join Us</a>
                <a href="Login.aspx" class="btn btn-secondary">Login</a>
            </div>
        </section>

        <!-- About Section -->
        <section id="about" class="section">
            <h2 class="section-title">About Us</h2>
            <div class="about-content">
                <div class="about-text">
                    <p>MOKI POINTS is a fun and engaging chore management system designed to help families track chores and reward achievements with points.</p>
                    <p style="margin-top: 20px;">Make household tasks enjoyable and motivate your family members to complete their chores while earning points along the way!</p>
                </div>
                <div class="video-container">
                    <iframe src="https://www.youtube.com/embed/Cm4JckG4n-k?start=25" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
                </div>
            </div>
        </section>

        <!-- Updates Section -->
        <section id="updates" class="section">
            <h2 class="section-title">Updates</h2>
            <div class="updates-content">
                <div class="update-card" onclick="openUpdateModal(1)">
                    <img src="Images/Landing/poster_1.png" alt="MOKI POINTS Introduction" class="update-image" />
                    <div class="update-content">
                        <div class="update-date">November 2024</div>
                        <div class="update-title">Welcome to MOKI POINTS!</div>
                        <p>Discover our innovative web application designed to transform how families manage chores and rewards. Learn what MOKI POINTS is all about and how it can benefit your family...</p>
                    </div>
                </div>
                <div class="update-card" onclick="openUpdateModal(2)">
                    <img src="Images/Landing/poster_2.png" alt="Learning Discipline & Family Fun" class="update-image" />
                    <div class="update-content">
                        <div class="update-date">November 2024</div>
                        <div class="update-title">Learning Discipline Made Fun</div>
                        <p>See how children can learn discipline while having fun with the whole family. MOKI POINTS makes household tasks enjoyable and rewarding for everyone...</p>
                    </div>
                </div>
                <div class="update-card" onclick="openUpdateModal(3)">
                    <img src="Images/Landing/poster_3.png" alt="Android App Coming Soon" class="update-image" />
                    <div class="update-content">
                        <div class="update-date">Coming Soon</div>
                        <div class="update-title">Android App Teaser</div>
                        <p>Get ready! Our Android mobile app is coming soon. Take MOKI POINTS with you wherever you go and manage your family's chores on the move...</p>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Update Modals -->
        <div id="updateModal1" class="update-modal" onclick="closeUpdateModalOnBackdrop(event, 1)">
            <div class="modal-content" onclick="event.stopPropagation();">
                <div class="modal-header">
                    <img src="Images/Landing/poster_1.png" alt="MOKI POINTS Introduction" class="modal-image" />
                    <span class="modal-close" onclick="closeUpdateModal(1)">&times;</span>
                </div>
                <div class="modal-body">
                    <div class="modal-date">November 2024</div>
                    <div class="modal-title">Welcome to MOKI POINTS!</div>
                    <div class="modal-text">
                        <p><strong>What is MOKI POINTS?</strong></p>
                        <p>MOKI POINTS is an innovative web application designed to revolutionize how families manage household chores and rewards. Our platform transforms the traditional chore system into an engaging, point-based experience that motivates both children and parents.</p>
                        <p><strong>Key Features:</strong></p>
                        <ul style="margin-left: 20px; margin-top: 10px;">
                            <li>Easy task creation and assignment for parents</li>
                            <li>Point-based reward system that motivates children</li>
                            <li>Family management with secure family codes</li>
                            <li>Task review and approval system</li>
                            <li>Real-time point tracking and history</li>
                            <li>User-friendly dashboard for both parents and children</li>
                        </ul>
                        <p><strong>Why MOKI POINTS?</strong></p>
                        <p>We understand that managing household chores can be challenging. MOKI POINTS makes it fun, fair, and rewarding. Children learn responsibility while earning points, and parents can easily track progress and manage tasks all in one place.</p>
                        <p>Join thousands of families who have transformed their household management with MOKI POINTS!</p>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="updateModal2" class="update-modal" onclick="closeUpdateModalOnBackdrop(event, 2)">
            <div class="modal-content" onclick="event.stopPropagation();">
                <div class="modal-header">
                    <img src="Images/Landing/poster_2.png" alt="Learning Discipline & Family Fun" class="modal-image" />
                    <span class="modal-close" onclick="closeUpdateModal(2)">&times;</span>
                </div>
                <div class="modal-body">
                    <div class="modal-date">November 2024</div>
                    <div class="modal-title">Learning Discipline Made Fun for the Whole Family</div>
                    <div class="modal-text">
                        <p><strong>Making Chores Fun and Educational</strong></p>
                        <p>At MOKI POINTS, we believe that learning discipline doesn't have to be boring or stressful. Our platform turns everyday household tasks into exciting opportunities for children to learn responsibility, time management, and the value of hard work.</p>
                        <p><strong>How It Works:</strong></p>
                        <ul style="margin-left: 20px; margin-top: 10px;">
                            <li><strong>Gamification:</strong> Tasks become challenges that children want to complete</li>
                            <li><strong>Point Rewards:</strong> Every completed task earns points, creating a sense of achievement</li>
                            <li><strong>Family Involvement:</strong> Parents and children work together, strengthening family bonds</li>
                            <li><strong>Progress Tracking:</strong> Visual progress helps children see their accomplishments</li>
                            <li><strong>Fair System:</strong> Transparent point values and review process ensure fairness</li>
                        </ul>
                        <p><strong>Benefits for Children:</strong></p>
                        <p>Children learn important life skills while having fun. They develop a sense of responsibility, understand the value of completing tasks, and experience the satisfaction of earning rewards through their efforts.</p>
                        <p><strong>Benefits for Families:</strong></p>
                        <p>Families enjoy a more organized household, reduced conflicts about chores, and quality time spent together. Parents can focus on teaching and bonding rather than constantly reminding children about tasks.</p>
                        <p>Transform your family's approach to chores with MOKI POINTS - where discipline meets fun!</p>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="updateModal3" class="update-modal" onclick="closeUpdateModalOnBackdrop(event, 3)">
            <div class="modal-content" onclick="event.stopPropagation();">
                <div class="modal-header">
                    <img src="Images/Landing/poster_3.png" alt="Android App Coming Soon" class="modal-image" />
                    <span class="modal-close" onclick="closeUpdateModal(3)">&times;</span>
                </div>
                <div class="modal-body">
                    <div class="modal-date">Coming Soon</div>
                    <div class="modal-title">MOKI POINTS Android App - Coming Soon!</div>
                    <div class="modal-text">
                        <p><strong>Take MOKI POINTS With You</strong></p>
                        <p>We're excited to announce that the MOKI POINTS Android mobile app is currently in development! Soon, you'll be able to manage your family's chores and track points from anywhere, anytime.</p>
                        <p><strong>What to Expect:</strong></p>
                        <ul style="margin-left: 20px; margin-top: 10px;">
                            <li><strong>Full Feature Access:</strong> All web app features available on your mobile device</li>
                            <li><strong>Push Notifications:</strong> Get notified when tasks are assigned or completed</li>
                            <li><strong>Quick Task Management:</strong> Create and assign tasks on the go</li>
                            <li><strong>Photo Uploads:</strong> Children can easily submit task completion photos</li>
                            <li><strong>Offline Mode:</strong> View your dashboard even without internet connection</li>
                            <li><strong>Family Sync:</strong> Real-time updates across all devices</li>
                        </ul>
                        <p><strong>Why Mobile?</strong></p>
                        <p>We understand that families are always on the move. With our Android app, parents can quickly assign tasks while at work, and children can check their progress and submit completed tasks from anywhere. The app brings the convenience of MOKI POINTS right to your pocket.</p>
                        <p><strong>Stay Tuned!</strong></p>
                        <p>We're working hard to bring you the best mobile experience possible. Follow our updates to be among the first to know when the app launches. The future of family chore management is coming to your Android device soon!</p>
                        <p style="margin-top: 20px; font-weight: bold; color: #0066CC;">Available on Google Play Store - Coming Soon!</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Contact Section -->
        <section id="contact" class="section">
            <h2 class="section-title">Contact Us</h2>
            <div class="contact-content">
                <p style="text-align: center; margin-bottom: 20px;">Have questions or feedback? We'd love to hear from you!</p>
                <div class="contact-details">
                    <div class="contact-info-section">
                        <h3>Get in Touch</h3>
                        <p><strong>Phone:</strong> <a href="tel:+63627941298">+63 62 794 1298</a></p>
                        <p><strong>Email:</strong> <a href="mailto:info.mokipoin@gmail.com">info.mokipoin@gmail.com</a></p>
                        <p style="margin-top: 30px;">Visit us at our location in Ormoc Centrum!</p>
                    </div>
                    <div class="map-container">
                        <iframe src="https://www.google.com/maps?q=Ormoc+Centrum&t=&z=19&ie=UTF8&iwloc=&output=embed" allowfullscreen="" loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>
                    </div>
                </div>
            </div>
        </section>

        <!-- Footer -->
        <footer id="footer">
            <div class="footer-content">
                <div class="footer-brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <p class="footer-text">&copy; 2024 MOKI POINTS. All rights reserved.</p>
            </div>
        </footer>
        </div>
    </form>
</body>
</html>
