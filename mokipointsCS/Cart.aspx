<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="mokipointsCS.Cart" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Shopping Cart - MOKI POINTS</title>
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
            background-color: #f5f5f5;
            color: #333;
        }
        
        /* Header */
        .header {
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 15px 0;
            margin-bottom: 30px;
        }
        
        .header-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .brand {
            font-size: 24px;
            font-weight: bold;
            letter-spacing: 3px;
        }
        
        .brand .moki {
            color: #0066CC;
        }
        
        .brand .points {
            color: #FF6600;
        }
        
        .nav-links {
            display: flex;
            gap: 20px;
            align-items: center;
        }
        
        .nav-links a {
            color: #333;
            text-decoration: none;
            font-weight: 500;
        }
        
        .nav-links a:hover, .nav-links a.active {
            color: #0066CC;
        }
        
        .user-info {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        
        .user-name {
            color: #333;
            font-weight: 500;
        }
        
        .points-display-header {
            background: linear-gradient(135deg, #FF6600 0%, #FF8533 100%);
            color: white;
            padding: 10px 20px;
            border-radius: 20px;
            font-weight: 600;
            font-size: 16px;
        }
        
        .btn-settings {
            padding: 8px 12px;
            background-color: transparent;
            color: #333;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            cursor: pointer;
            text-decoration: none;
            font-size: 18px;
            transition: all 0.3s;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 40px;
            height: 40px;
        }
        
        .btn-settings:hover {
            background-color: #f5f5f5;
            border-color: #0066CC;
            color: #0066CC;
        }
        
        .hamburger-icon {
            display: flex;
            flex-direction: column;
            gap: 4px;
            width: 20px;
        }
        
        .hamburger-line {
            width: 100%;
            height: 3px;
            background-color: currentColor;
            border-radius: 2px;
        }
        
        /* Profile Picture Avatar */
        .profile-avatar {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            object-fit: cover;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 12px;
        }
        
        .profile-avatar:hover {
            border-color: #FF6600;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(255, 102, 0, 0.3);
        }
        
        .profile-avatar-placeholder {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            background: linear-gradient(135deg, #FF6600 0%, #e55a00 100%);
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 12px;
            text-decoration: none;
        }
        
        .profile-avatar-placeholder:hover {
            border-color: #FF6600;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(255, 102, 0, 0.3);
        }
        
        /* Container */
        .container {
            max-width: 1000px;
            margin: 0 auto;
            padding: 0 30px 30px;
        }
        
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
        }
        
        .btn-continue-shopping {
            padding: 10px 20px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }
        
        .btn-continue-shopping:hover {
            background-color: #5a6268;
        }
        
        /* Message Container */
        .message-container {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            max-width: 400px;
        }
        
        .message {
            padding: 15px 20px;
            margin-bottom: 10px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            display: flex;
            align-items: center;
            animation: slideIn 0.3s ease-out;
        }
        
        .message-success {
            background-color: #d4edda;
            border-left: 4px solid #28a745;
            color: #155724;
        }
        
        .message-error {
            background-color: #f8d7da;
            border-left: 4px solid #dc3545;
            color: #721c24;
        }
        
        .message-icon {
            font-size: 20px;
            margin-right: 10px;
            font-weight: bold;
        }
        
        .message-text {
            flex: 1;
            font-size: 14px;
        }
        
        .message-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            margin-left: 10px;
            opacity: 0.7;
        }
        
        .message-close:hover {
            opacity: 1;
        }
        
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }
        
        /* Cart Items */
        .cart-items {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        
        .cart-item {
            display: flex;
            gap: 20px;
            padding: 20px;
            border-bottom: 1px solid #e0e0e0;
            align-items: center;
        }
        
        .cart-item:last-child {
            border-bottom: none;
        }
        
        .cart-item-image {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 5px;
            background-color: #f0f0f0;
        }
        
        .cart-item-details {
            flex: 1;
        }
        
        .cart-item-name {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }
        
        .cart-item-points {
            font-size: 16px;
            color: #FF6600;
            font-weight: 600;
        }
        
        .cart-item-quantity {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .quantity-controls {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .btn-quantity {
            width: 30px;
            height: 30px;
            border: 1px solid #ddd;
            background: white;
            border-radius: 5px;
            cursor: pointer;
            font-size: 18px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .btn-quantity:hover {
            background: #f5f5f5;
        }
        
        .quantity-input {
            width: 60px;
            padding: 5px;
            text-align: center;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
        }
        
        .cart-item-subtotal {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            min-width: 100px;
            text-align: right;
        }
        
        .btn-remove {
            padding: 8px 16px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
        }
        
        .btn-remove:hover {
            background-color: #c82333;
        }
        
        /* Cart Summary */
        .cart-summary {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
            font-size: 16px;
        }
        
        .summary-row.total {
            border-top: 2px solid #e0e0e0;
            padding-top: 15px;
            margin-top: 15px;
            font-size: 24px;
            font-weight: 600;
            color: #FF6600;
        }
        
        .btn-checkout {
            width: 100%;
            padding: 15px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 18px;
            font-weight: 600;
            margin-top: 20px;
            transition: all 0.3s;
        }
        
        .btn-checkout:hover {
            background-color: #0052a3;
        }
        
        .btn-checkout:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }
        
        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #999;
            background: white;
            border-radius: 10px;
        }
        
        .empty-state h3 {
            font-size: 24px;
            margin-bottom: 10px;
            color: #666;
        }
        
        .insufficient-points {
            background: #fff3cd;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            color: #856404;
            border-left: 4px solid #ffc107;
        }
        
        /* Checkout Confirmation Modal */
        .checkout-modal {
            display: none;
            position: fixed;
            z-index: 10000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.6);
            animation: fadeIn 0.3s ease;
        }
        
        .checkout-modal.active {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }
        
        .checkout-modal-content {
            background-color: white;
            margin: auto;
            padding: 0;
            border-radius: 15px;
            max-width: 500px;
            width: 100%;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
            animation: slideDown 0.4s ease;
            overflow: hidden;
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
        
        .checkout-modal-header {
            background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%);
            padding: 30px;
            text-align: center;
            position: relative;
            color: white;
        }
        
        .checkout-modal-icon {
            font-size: 48px;
            margin-bottom: 15px;
            animation: bounceIn 0.6s ease-out;
        }
        
        @keyframes bounceIn {
            0% {
                transform: scale(0);
                opacity: 0;
            }
            50% {
                transform: scale(1.2);
            }
            100% {
                transform: scale(1);
                opacity: 1;
            }
        }
        
        .checkout-modal-title {
            color: white;
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }
        
        .checkout-modal-close {
            position: absolute;
            top: 15px;
            right: 15px;
            color: white;
            font-size: 32px;
            font-weight: bold;
            background: none;
            border: none;
            cursor: pointer;
            width: 40px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: background-color 0.3s;
        }
        
        .checkout-modal-close:hover {
            background-color: rgba(255, 255, 255, 0.2);
        }
        
        .checkout-modal-body {
            padding: 30px;
            max-height: 60vh;
            overflow-y: auto;
        }
        
        .checkout-modal-message {
            font-size: 16px;
            color: #333;
            margin-bottom: 20px;
            text-align: center;
            line-height: 1.6;
        }
        
        .checkout-order-summary {
            background-color: #f8f9fa;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
        }
        
        .checkout-summary-title {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            margin-bottom: 15px;
            border-bottom: 2px solid #e0e0e0;
            padding-bottom: 10px;
        }
        
        .checkout-items-list {
            margin-bottom: 15px;
        }
        
        .checkout-item-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #e0e0e0;
            font-size: 14px;
        }
        
        .checkout-item-row:last-child {
            border-bottom: none;
        }
        
        .checkout-item-name {
            color: #333;
            flex: 1;
        }
        
        .checkout-item-quantity {
            color: #666;
            margin: 0 10px;
        }
        
        .checkout-item-cost {
            color: #FF6600;
            font-weight: 600;
            min-width: 100px;
            text-align: right;
        }
        
        .checkout-summary-total {
            margin-top: 15px;
            padding-top: 15px;
            border-top: 2px solid #e0e0e0;
        }
        
        .checkout-summary-total .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            font-size: 16px;
        }
        
        .checkout-summary-total .summary-row.total {
            font-size: 20px;
            font-weight: 600;
            color: #FF6600;
            margin-top: 10px;
            padding-top: 10px;
            border-top: 2px solid #e0e0e0;
        }
        
        .checkout-warning {
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            border-radius: 5px;
            color: #856404;
            font-size: 14px;
            line-height: 1.6;
        }
        
        .checkout-modal-footer {
            padding: 20px 30px;
            background-color: #f8f9fa;
            display: flex;
            gap: 15px;
            justify-content: flex-end;
            border-top: 1px solid #e0e0e0;
        }
        
        .checkout-btn {
            padding: 12px 30px;
            font-size: 16px;
            font-weight: 600;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s ease;
            min-width: 120px;
        }
        
        .checkout-btn-cancel {
            background-color: #e0e0e0;
            color: #333;
        }
        
        .checkout-btn-cancel:hover {
            background-color: #d0d0d0;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .checkout-btn-confirm {
            background: linear-gradient(135deg, #0066CC 0%, #0052a3 100%);
            color: white;
        }
        
        .checkout-btn-confirm:hover {
            background: linear-gradient(135deg, #0052a3 0%, #0066CC 100%);
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 102, 204, 0.4);
        }
        
        .checkout-btn-confirm:active {
            transform: translateY(0);
        }
    </style>
    <script>
        // Message functions
        function showMessage(type, message) {
            const messageId = type + 'Message';
            const messageElement = document.getElementById(messageId);
            if (messageElement) {
                const textElement = messageElement.querySelector('.message-text');
                if (textElement) {
                    textElement.textContent = message;
                    messageElement.style.display = 'flex';
                    
                    const hideDelay = type === 'error' ? 7000 : 5000;
                    setTimeout(() => {
                        closeMessage(messageId);
                    }, hideDelay);
                }
            }
        }
        
        function closeMessage(messageId) {
            const messageElement = document.getElementById(messageId);
            if (messageElement) {
                messageElement.style.animation = 'slideOut 0.3s ease-out';
                setTimeout(() => {
                    messageElement.style.display = 'none';
                    messageElement.style.animation = 'slideIn 0.3s ease-out';
                }, 300);
            }
        }
        
        // Update quantity
        function updateQuantity(rewardId, change) {
            // Try to find by ID first (if ClientIDMode.Static works)
            var quantityInput = document.getElementById('quantity_' + rewardId);
            
            // If not found, try using data-reward-id attribute (fallback for Repeater controls)
            if (!quantityInput) {
                var quantityInputs = document.querySelectorAll('input[data-reward-id="' + rewardId + '"]');
                quantityInput = quantityInputs.length > 0 ? quantityInputs[0] : null;
            }
            
            if (!quantityInput) {
                console.error('Quantity input not found for rewardId: ' + rewardId);
                // Debug: log all quantity inputs to help troubleshoot
                var allInputs = document.querySelectorAll('.quantity-input');
                console.log('Found ' + allInputs.length + ' quantity inputs on page');
                allInputs.forEach(function(input, index) {
                    console.log('Input ' + index + ': id=' + input.id + ', data-reward-id=' + input.getAttribute('data-reward-id'));
                });
                return;
            }
            
            var currentQty = parseInt(quantityInput.value) || 1;
            var newQty = currentQty + change;
            
            // Prevent going below 1
            if (newQty < 1) {
                newQty = 1;
            }
            
            // Note: Full validation happens server-side
            // Client-side just prevents obviously invalid values
            
            quantityInput.value = newQty;
            
            // Update the cart summary immediately (client-side)
            updateCartSummary();
            
            // Trigger postback to update (server will validate)
            // Use form submission - find the form element
            var form = document.getElementById('<%= form1.ClientID %>') || document.forms[0] || document.querySelector('form');
            if (form) {
                // Remove any existing hidden inputs first
                var existingTarget = document.getElementById('__EVENTTARGET');
                var existingArgument = document.getElementById('__EVENTARGUMENT');
                if (existingTarget) existingTarget.remove();
                if (existingArgument) existingArgument.remove();
                
                // Create hidden inputs for the postback
                var eventTarget = document.createElement('input');
                eventTarget.type = 'hidden';
                eventTarget.id = '__EVENTTARGET';
                eventTarget.name = '__EVENTTARGET';
                eventTarget.value = 'UpdateQuantity';
                form.appendChild(eventTarget);
                
                var eventArgument = document.createElement('input');
                eventArgument.type = 'hidden';
                eventArgument.id = '__EVENTARGUMENT';
                eventArgument.name = '__EVENTARGUMENT';
                eventArgument.value = rewardId + '|' + newQty;
                form.appendChild(eventArgument);
                
                form.submit();
            } else {
                console.error('Form not found');
            }
        }
        
        // Update cart summary (client-side calculation)
        function updateCartSummary() {
            var totalPoints = 0;
            var cartItemElements = document.querySelectorAll('.cart-item');
            
            cartItemElements.forEach(function(item) {
                var pointsElement = item.querySelector('.cart-item-points');
                var quantityInput = item.querySelector('.quantity-input');
                
                if (pointsElement && quantityInput) {
                    var pointsText = pointsElement.textContent.trim();
                    var pointCost = parseInt(pointsText.replace(/[^0-9]/g, '')) || 0;
                    var quantity = parseInt(quantityInput.value) || 1;
                    totalPoints += pointCost * quantity;
                }
            });
            
            // Update subtotal and total displays
            var subtotalElements = document.querySelectorAll('.cart-summary .summary-row span:last-child');
            if (subtotalElements.length >= 2) {
                subtotalElements[0].textContent = totalPoints.toLocaleString() + ' points';
                subtotalElements[1].textContent = totalPoints.toLocaleString() + ' points';
            }
        }
        
        // Add validation for manual input changes
        function validateQuantityInput(input) {
            var rewardId = input.getAttribute('data-reward-id');
            var quantity = parseInt(input.value) || 1;
            
            if (quantity < 1) {
                input.value = 1;
                quantity = 1;
            }
            
            // Trigger postback for server-side validation
            __doPostBack('UpdateQuantity', rewardId + '|' + quantity);
        }
        
        // Checkout Confirmation Modal Functions
        function showCheckoutConfirmation() {
            try {
                var modal = document.getElementById('checkoutModal');
                if (!modal) {
                    console.error('Checkout modal not found');
                    return false;
                }
                
                // Get cart items from page
                var cartItems = [];
                var totalPoints = 0;
                
                // Extract items from cart display (from Repeater)
                var cartItemElements = document.querySelectorAll('.cart-item');
                cartItemElements.forEach(function(item) {
                    var nameElement = item.querySelector('.cart-item-name');
                    var pointsElement = item.querySelector('.cart-item-points');
                    var quantityInput = item.querySelector('.quantity-input');
                    var subtotalElement = item.querySelector('.cart-item-subtotal');
                    
                    if (nameElement && pointsElement && quantityInput && subtotalElement) {
                        var name = nameElement.textContent.trim();
                        var pointsText = pointsElement.textContent.trim();
                        var pointCost = parseInt(pointsText.replace(/[^0-9]/g, '')) || 0;
                        var quantity = parseInt(quantityInput.value) || 1;
                        var subtotal = pointCost * quantity;
                        
                        cartItems.push({
                            name: name,
                            pointCost: pointCost,
                            quantity: quantity,
                            subtotal: subtotal
                        });
                        
                        totalPoints += subtotal;
                    }
                });
                
                // Populate modal with items
                var itemsList = document.getElementById('checkoutItemsList');
                if (itemsList) {
                    itemsList.innerHTML = '';
                    
                    if (cartItems.length === 0) {
                        itemsList.innerHTML = '<p style="color: #999; text-align: center; padding: 20px;">No items in cart</p>';
                    } else {
                        cartItems.forEach(function(item) {
                            var itemRow = document.createElement('div');
                            itemRow.className = 'checkout-item-row';
                            itemRow.innerHTML = 
                                '<span class="checkout-item-name">' + escapeHtml(item.name) + '</span>' +
                                '<span class="checkout-item-quantity">x ' + item.quantity + '</span>' +
                                '<span class="checkout-item-cost">' + item.subtotal.toLocaleString() + ' pts</span>';
                            itemsList.appendChild(itemRow);
                        });
                    }
                }
                
                // Update totals
                var subtotalElement = document.getElementById('checkoutSubtotal');
                var totalElement = document.getElementById('checkoutTotal');
                
                if (subtotalElement) {
                    subtotalElement.textContent = totalPoints.toLocaleString() + ' points';
                }
                if (totalElement) {
                    totalElement.textContent = totalPoints.toLocaleString() + ' points';
                }
                
                // Show modal
                modal.classList.add('active');
                document.body.style.overflow = 'hidden';
                
                console.log('Checkout confirmation modal shown');
                return false; // Prevent default button action
            } catch (e) {
                console.error('Error showing checkout confirmation:', e);
                // Fallback: proceed with checkout if modal fails
                return true; // Allow default action
            }
        }
        
        function closeCheckoutModal() {
            var modal = document.getElementById('checkoutModal');
            if (modal) {
                modal.classList.remove('active');
                document.body.style.overflow = 'auto';
            }
        }
        
        function closeCheckoutModalOnBackdrop(event) {
            if (event.target.classList.contains('checkout-modal')) {
                closeCheckoutModal();
            }
        }
        
        function confirmCheckout() {
            try {
                // Hide modal first
                closeCheckoutModal();
                
                // Small delay to ensure modal is closed
                setTimeout(function() {
                    // Trigger server-side checkout using form submission
                    var form = document.getElementById('<%= form1.ClientID %>') || document.forms[0] || document.querySelector('form');
                    if (form) {
                        // Remove any existing hidden inputs first
                        var existingTarget = document.getElementById('__EVENTTARGET_CHECKOUT');
                        if (existingTarget) existingTarget.remove();
                        
                        // Create hidden input to trigger the button click
                        var eventTarget = document.createElement('input');
                        eventTarget.type = 'hidden';
                        eventTarget.id = '__EVENTTARGET_CHECKOUT';
                        eventTarget.name = '__EVENTTARGET';
                        // Use the button's UniqueID to trigger its click event
                        eventTarget.value = '<%= btnCheckout.UniqueID %>';
                        form.appendChild(eventTarget);
                        
                        // Submit the form to trigger the server-side click event
                        form.submit();
                    } else {
                        console.error('Form not found');
                        showMessage('error', 'Error: Could not process checkout. Please try again.');
                    }
                }, 100);
            } catch (e) {
                console.error('Error confirming checkout:', e);
                showMessage('error', 'An error occurred. Please try again.');
            }
        }
        
        // HTML escape function to prevent XSS and encoding issues
        function escapeHtml(text) {
            if (!text) return '';
            var map = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return text.toString().replace(/[&<>"']/g, function(m) { return map[m]; });
        }
        
        // Close modal with Escape key
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                var modal = document.getElementById('checkoutModal');
                if (modal && modal.classList.contains('active')) {
                    closeCheckoutModal();
                }
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Message Container -->
        <div class="message-container">
            <div class="message message-success" id="successMessage" style="display: none;">
                <span class="message-icon">&#10003;</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('successMessage')">&#215;</button>
            </div>
            <div class="message message-error" id="errorMessage" style="display: none;">
                <span class="message-icon">&#10007;</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('errorMessage')">&#215;</button>
            </div>
        </div>
        
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="ChildDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="ChildTasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="PointsHistory.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Points</a>
                        <a href="RewardShop.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Shop</a>
                        <a href="Cart.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Cart</a>
                        <a href="MyOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">My Orders</a>
                    </div>
                    <a href="Profile.aspx" style="text-decoration: none; display: flex; align-items: center;">
                        <asp:Image ID="imgProfilePicture" runat="server" CssClass="profile-avatar" Visible="false" />
                        <asp:Literal ID="litProfilePlaceholder" runat="server"></asp:Literal>
                    </a>
                    <span class="user-name"><asp:Literal ID="litUserName" runat="server"></asp:Literal></span>
                    <a href="Settings.aspx" class="btn-settings" title="Settings">
                        <div class="hamburger-icon">
                            <div class="hamburger-line"></div>
                            <div class="hamburger-line"></div>
                            <div class="hamburger-line"></div>
                        </div>
                    </a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <div class="page-header">
                <h1 class="page-title">Shopping Cart</h1>
                <a href="RewardShop.aspx" class="btn-continue-shopping">Continue Shopping</a>
            </div>

            <asp:Panel ID="pnlCart" runat="server">
                <div class="cart-items">
                    <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                        <ItemTemplate>
                            <div class="cart-item">
                                <%# Eval("ImageUrl") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? "<img src='" + Eval("ImageUrl") + "' alt='" + Server.HtmlEncode(Eval("Name").ToString()) + "' class='cart-item-image' />" : "<div class='cart-item-image'></div>" %>
                                
                                <div class="cart-item-details">
                                    <div class="cart-item-name"><%# Eval("Name") %></div>
                                    <div class="cart-item-points"><%# Eval("PointCost") %> points each</div>
                                </div>
                                
                                <div class="cart-item-quantity">
                                    <div class="quantity-controls">
                                        <button type="button" class="btn-quantity" onclick='updateQuantity(<%# Eval("RewardId") %>, -1)'>-</button>
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="quantity-input" Text='<%# Eval("Quantity") %>' 
                                            data-reward-id='<%# Eval("RewardId") %>' onchange='validateQuantityInput(this)' />
                                        <button type="button" class="btn-quantity" onclick='updateQuantity(<%# Eval("RewardId") %>, 1)'>+</button>
                                    </div>
                                </div>
                                
                                <div class="cart-item-subtotal">
                                    <asp:Literal ID="litSubtotal" runat="server"></asp:Literal>
                                </div>
                                
                                <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn-remove" 
                                    CommandName="Remove" CommandArgument='<%# Eval("RewardId") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="cart-summary">
                    <div class="summary-row">
                        <span>Subtotal:</span>
                        <span><asp:Literal ID="litSubtotal" runat="server"></asp:Literal></span>
                    </div>
                    <div class="summary-row total">
                        <span>Total:</span>
                        <span><asp:Literal ID="litTotal" runat="server"></asp:Literal></span>
                    </div>
                    
                    <asp:Panel ID="pnlInsufficientPoints" runat="server" Visible="false" CssClass="insufficient-points">
                        You don't have enough points for this purchase. You need <asp:Literal ID="litPointsNeeded" runat="server"></asp:Literal> more points.
                    </asp:Panel>
                    
                    <asp:Button ID="btnCheckout" runat="server" Text="Checkout" CssClass="btn-checkout" 
                        OnClientClick="return showCheckoutConfirmation();" OnClick="btnCheckout_Click" />
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>Your cart is empty</h3>
                    <p>Add some rewards to your cart to get started!</p>
                    <a href="RewardShop.aspx" class="btn-continue-shopping" style="margin-top: 20px;">Continue Shopping</a>
                </div>
            </asp:Panel>
        </div>
        
        <!-- Checkout Confirmation Modal -->
        <div id="checkoutModal" class="checkout-modal" onclick="closeCheckoutModalOnBackdrop(event);">
            <div class="checkout-modal-content" onclick="event.stopPropagation();">
                <div class="checkout-modal-header">
                    <div class="checkout-modal-icon">&#128722;</div>
                    <h2 class="checkout-modal-title">Confirm Your Order</h2>
                    <button type="button" class="checkout-modal-close" onclick="closeCheckoutModal();">&times;</button>
                </div>
                <div class="checkout-modal-body">
                    <p class="checkout-modal-message">Are you sure you want to proceed with this purchase?</p>
                    
                    <!-- Order Summary -->
                    <div class="checkout-order-summary">
                        <h3 class="checkout-summary-title">Order Summary</h3>
                        <div id="checkoutItemsList" class="checkout-items-list">
                            <!-- Items will be populated via JavaScript -->
                        </div>
                        <div class="checkout-summary-total">
                            <div class="summary-row">
                                <span>Subtotal:</span>
                                <span id="checkoutSubtotal">0 points</span>
                            </div>
                            <div class="summary-row total">
                                <span>Total:</span>
                                <span id="checkoutTotal">0 points</span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="checkout-warning">
                        <strong>Note:</strong> This order will be sent to your parent for approval. Points will be deducted after parent confirms.
                    </div>
                </div>
                <div class="checkout-modal-footer">
                    <button type="button" class="checkout-btn checkout-btn-cancel" onclick="closeCheckoutModal();">Cancel</button>
                    <asp:Button ID="btnConfirmCheckout" runat="server" Text="Confirm Checkout" 
                        CssClass="checkout-btn checkout-btn-confirm" OnClick="btnCheckout_Click" style="display: none;" />
                    <button type="button" class="checkout-btn checkout-btn-confirm" onclick="confirmCheckout();">Confirm Checkout</button>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

