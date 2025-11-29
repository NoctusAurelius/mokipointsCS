<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Family.aspx.cs" Inherits="mokipointsCS.Family" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Family - MOKI POINTS</title>
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
        
        .nav-links a:hover {
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
            border-color: #0066CC;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
        }
        
        .profile-avatar-placeholder {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            background: linear-gradient(135deg, #0066CC 0%, #0052a3 100%);
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
            border-color: #0066CC;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
        }
        
        /* Container */
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 0 30px;
        }

        /* Override container max-width when showing family info (with chat) */
        .container .family-layout,
        .container .pnlFamilyInfo {
            max-width: 100% !important;
            margin: 0 !important;
            padding: 0 !important;
            width: 100% !important;
        }
        
        /* Force full width for family layout and main area */
        .family-layout {
            width: 100% !important;
            max-width: 100% !important;
        }
        
        .family-main {
            width: calc(100vw - 620px) !important; /* Full viewport minus sidebars */
            max-width: none !important;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 10px;
        }
        
        .page-subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 30px;
        }
        
        /* Tabs */
        .tabs {
            display: flex;
            gap: 10px;
            margin-bottom: 30px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .tab {
            padding: 15px 30px;
            background: none;
            border: none;
            font-size: 16px;
            font-weight: bold;
            color: #666;
            cursor: pointer;
            border-bottom: 3px solid transparent;
            transition: all 0.3s;
        }
        
        .tab.active {
            color: #0066CC;
            border-bottom-color: #0066CC;
        }
        
        .tab:hover {
            color: #0066CC;
        }
        
        /* Forms */
        .form-container {
            background-color: white;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .form-group {
            margin-bottom: 25px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: 500;
            font-size: 14px;
        }
        
        .form-group label .required {
            color: #d32f2f;
        }
        
        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            font-size: 16px;
            transition: border-color 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
        }
        
        .btn-submit {
            width: 100%;
            padding: 14px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s;
            margin-top: 10px;
        }
        
        .btn-submit:hover {
            background-color: #0052a3;
        }
        
        .btn-submit:disabled {
            background-color: #999;
            cursor: not-allowed;
        }
        
        /* Family Info Card */
        .family-info-card {
            background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%);
            border-radius: 15px;
            padding: 40px;
            color: white;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        }
        
        .family-name {
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 15px;
        }
        
        .family-code-section {
            background-color: rgba(255,255,255,0.2);
            border-radius: 10px;
            padding: 20px;
            margin-top: 20px;
        }
        
        .code-label {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 10px;
        }
        
        .family-code {
            font-size: 48px;
            font-weight: bold;
            letter-spacing: 4px;
            font-family: 'Courier New', monospace;
        }
        
        .code-description {
            font-size: 14px;
            opacity: 0.8;
            margin-top: 10px;
        }
        
        .treasury-info {
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid rgba(255,255,255,0.3);
        }
        
        .treasury-label {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 5px;
        }
        
        .treasury-amount {
            font-size: 36px;
            font-weight: bold;
        }
        
        .error-message {
            color: #d32f2f;
            font-size: 14px;
            margin-top: 10px;
            text-align: center;
            padding: 10px;
            background-color: #ffebee;
            border-radius: 5px;
            border-left: 3px solid #d32f2f;
        }
        
        .success-message {
            color: #2e7d32;
            font-size: 14px;
            margin-top: 10px;
            text-align: center;
            padding: 10px;
            background-color: #e8f5e9;
            border-radius: 5px;
            border-left: 3px solid #2e7d32;
        }
        
        .hidden {
            display: none;
        }

        /* Toast Message - Compact */
        .message-toast {
            position: fixed !important;
            top: 20px !important;
            right: 20px !important;
            left: auto !important;
            bottom: auto !important;
            background-color: white !important;
            border-radius: 6px !important;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15) !important;
            padding: 8px 12px !important;
            width: fit-content !important;
            max-width: 220px !important;
            min-width: 0 !important;
            z-index: 10000 !important;
            display: inline-flex !important;
            align-items: center !important;
            gap: 8px !important;
            opacity: 0;
            transform: translateX(400px);
            transition: all 0.3s ease;
            pointer-events: none;
            box-sizing: border-box !important;
            height: fit-content !important;
            min-height: auto !important;
            max-height: 45px !important;
            line-height: 1.3 !important;
            margin: 0 !important;
            flex-direction: row !important;
            flex-wrap: nowrap !important;
            overflow: hidden !important;
        }

        .message-toast.show {
            opacity: 1;
            transform: translateX(0);
            pointer-events: auto;
        }

        .message-toast.success {
            border-left: 3px solid #2e7d32;
        }

        .message-toast.error {
            border-left: 3px solid #d32f2f;
        }

        .message-toast-text {
            flex: 0 0 auto !important;
            font-size: 13px !important;
            color: #333;
            line-height: 1.3 !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
            max-width: 180px !important;
            padding: 0 !important;
            margin: 0 !important;
            height: auto !important;
            max-height: 22px !important;
            display: inline-block !important;
            vertical-align: middle !important;
        }

        .message-toast.success .message-toast-text {
            color: #2e7d32;
        }

        .message-toast.error .message-toast-text {
            color: #d32f2f;
        }

        .message-toast-close {
            background: none !important;
            border: none !important;
            font-size: 16px !important;
            color: #999;
            cursor: pointer;
            padding: 0 !important;
            width: 16px !important;
            height: 16px !important;
            min-width: 16px !important;
            max-width: 16px !important;
            display: flex !important;
            align-items: center;
            justify-content: center;
            line-height: 1 !important;
            transition: color 0.2s;
            flex-shrink: 0 !important;
            margin: 0 !important;
        }

        .message-toast-close:hover {
            color: #333;
        }

        /* Sidebar Layout - Discord Style (Left and Right Sidebars, Fixed) */
        .family-layout {
            display: flex;
            gap: 30px;
            margin-top: 30px;
            position: relative;
        }

        /* Left Sidebar - Invite Code */
        .family-sidebar-left {
            width: 280px;
            background-color: white;
            border-radius: 0;
            box-shadow: 2px 0 8px rgba(0,0,0,0.1);
            padding: 20px;
            height: calc(100vh - 80px);
            overflow-y: auto;
            flex-shrink: 0;
            position: fixed;
            left: 0;
            top: 80px;
            z-index: 100;
            border-right: 1px solid #e0e0e0;
        }

        .code-section-content {
            padding: 10px 0;
        }

        .code-section-content .code-label {
            font-size: 12px;
            color: #666;
            margin-bottom: 10px;
            font-weight: 500;
        }

        .code-section-content .code-display {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 10px;
            flex-wrap: wrap;
        }

        .code-section-content .family-code {
            font-size: 18px;
            font-weight: bold;
            color: #0066CC;
            letter-spacing: 2px;
            flex: 1;
            min-width: 120px;
        }

        .code-section-content .btn-copy-code,
        .code-section-content .btn-change-code {
            padding: 8px 16px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.2s;
        }

        .code-section-content .btn-copy-code {
            background-color: #0066CC;
            color: white;
        }

        .code-section-content .btn-copy-code:hover {
            background-color: #0052a3;
        }

        .code-section-content .btn-change-code {
            background-color: #FF6600;
            color: white;
        }

        .code-section-content .btn-change-code:hover {
            background-color: #e55a00;
        }

        .code-section-content .code-description {
            font-size: 12px;
            color: #999;
            margin-top: 5px;
        }

        /* Main Content Area - Chat */
        .family-main {
            flex: 1;
            margin-left: 310px; /* Left sidebar width + gap */
            margin-right: 310px; /* Right sidebar width + gap */
            display: flex;
            flex-direction: column;
            height: calc(100vh - 80px);
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        /* Chat Container */
        .chat-container {
            display: flex;
            flex-direction: column;
            height: 100%;
            background-color: #f5f5f5;
        }

        /* Chat Messages Area */
        .chat-messages {
            flex: 1;
            overflow-y: auto;
            padding: 20px;
            display: flex;
            flex-direction: column;
            gap: 15px;
        }

        .chat-messages::-webkit-scrollbar {
            width: 8px;
        }

        .chat-messages::-webkit-scrollbar-track {
            background: #f1f1f1;
        }

        .chat-messages::-webkit-scrollbar-thumb {
            background: #888;
            border-radius: 4px;
        }

        .chat-messages::-webkit-scrollbar-thumb:hover {
            background: #555;
        }

        .chat-loading, .chat-empty {
            text-align: center;
            padding: 40px;
            color: #999;
        }

        /* Chat Container - Full Width */
        .chat-container {
            display: flex;
            flex-direction: column;
            height: calc(100vh - 140px); /* Full height minus header and margins */
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
            max-width: 100%;
            width: 100%;
        }

        /* Chat Header with Family Name */
        .chat-header {
            background-color: white;
            border-bottom: 1px solid #e0e0e0;
            padding: 15px 20px;
            flex-shrink: 0;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .chat-family-name {
            font-size: 24px;
            font-weight: bold;
            color: #333;
            margin: 0;
            padding: 0;
        }

        /* Chat Messages Area - Scrollable */
        .chat-messages {
            flex: 1;
            overflow-y: auto;
            overflow-x: hidden;
            padding: 20px;
            display: flex;
            flex-direction: column;
            gap: 15px;
            background-color: #f8f9fa;
        }

        .empty-state {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 15px;
        }

        .empty-icon {
            font-size: 48px;
        }

        .empty-text {
            font-size: 16px;
            color: #666;
        }

        /* Chat Container - Full Width */
        .chat-container {
            display: flex;
            flex-direction: column;
            height: calc(100vh - 140px); /* Full height minus header and margins */
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
            max-width: 100%;
            width: 100%;
        }

        /* Chat Messages Area - Scrollable */
        .chat-messages {
            flex: 1;
            overflow-y: auto;
            overflow-x: hidden;
            padding: 20px;
            display: flex;
            flex-direction: column;
            gap: 15px;
            background-color: #f8f9fa;
        }

        .chat-loading {
            text-align: center;
            padding: 40px;
            color: #666;
            font-size: 14px;
        }

        .chat-empty {
            text-align: center;
            padding: 60px 20px;
        }

        /* Chat Message Bubble */
        .chat-message {
            display: flex;
            gap: 12px;
            max-width: 65%;
            width: fit-content;
            animation: fadeIn 0.3s ease-in;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @keyframes slideDown {
            from { 
                opacity: 0; 
                transform: translateY(-20px) scale(0.95); 
            }
            to { 
                opacity: 1; 
                transform: translateY(0) scale(1); 
            }
        }

        @keyframes bounceIn {
            0% { 
                opacity: 0; 
                transform: translateY(-30px) scale(0.8); 
            }
            50% { 
                opacity: 1; 
                transform: translateY(5px) scale(1.05); 
            }
            100% { 
                opacity: 1; 
                transform: translateY(0) scale(1); 
            }
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.1); }
        }

        .message-enter {
            animation: slideDown 0.4s ease-out;
        }

        .system-message-enter {
            animation: bounceIn 0.6s ease-out;
        }

        .system-message-enter .chat-message-bubble {
            animation: pulse 0.6s ease-out 0.3s;
        }

        .system-emoji {
            font-size: 1.2em;
            display: inline-block;
            animation: pulse 1s ease-in-out 0.5s 2;
        }

        .chat-message.own-message {
            align-self: flex-end;
            flex-direction: row-reverse;
        }

        .chat-message.system-message {
            align-self: center;
            max-width: 90%;
            justify-content: center;
        }

        .chat-message-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            overflow: hidden;
            flex-shrink: 0;
            background-color: #e0e0e0;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .chat-message-avatar img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .chat-message-content {
            flex: 1;
            display: flex;
            flex-direction: column;
            gap: 5px;
        }

        .chat-message-header {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 4px;
        }

        .chat-message-name {
            font-weight: 600;
            font-size: 14px;
            color: #333;
        }

        .chat-message-time {
            font-size: 11px;
            color: #999;
        }

        .chat-message-bubble {
            background-color: #e9ecef;
            padding: 10px 15px;
            border-radius: 18px;
            word-wrap: break-word;
            position: relative;
            display: inline-block;
            width: fit-content;
            max-width: 100%;
        }

        .chat-message.own-message .chat-message-bubble {
            background-color: #0066CC;
            color: white;
        }

        .chat-message.system-message .chat-message-bubble {
            background-color: #fff3cd;
            border: 1px solid #ffc107;
            color: #856404;
        }

        .chat-message.reward-message .chat-message-bubble {
            background-color: #d1ecf1;
            border: 1px solid #0dcaf0;
            color: #055160;
        }

        .chat-message-text {
            font-size: 14px;
            line-height: 1.4;
            white-space: pre-wrap;
        }

        .chat-message-image {
            max-width: 100%;
            border-radius: 8px;
            margin-top: 5px;
            cursor: pointer;
        }

        .chat-message-gif {
            max-width: 100%;
            border-radius: 8px;
            margin-top: 5px;
        }

        /* Reply Preview */
        .chat-reply-preview {
            border-left: 3px solid #0066CC;
            background-color: #f0f0f0;
            padding: 8px 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }

        .reply-preview-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 4px;
        }

        .reply-preview-name {
            font-size: 12px;
            font-weight: 600;
            color: #0066CC;
        }

        .reply-preview-close {
            background: none;
            border: none;
            font-size: 18px;
            color: #999;
            cursor: pointer;
            padding: 0;
            width: 20px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .reply-preview-text {
            font-size: 12px;
            color: #666;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        /* Chat Input Area */
        .chat-input-container {
            border-top: 1px solid #e0e0e0;
            background-color: white;
            padding: 15px;
        }

        .chat-input-wrapper {
            display: flex;
            align-items: flex-end;
            gap: 10px;
        }

        .chat-btn {
            width: 40px;
            height: 40px;
            border: none;
            border-radius: 50%;
            background-color: #f0f0f0;
            color: #666;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            transition: all 0.2s;
            flex-shrink: 0;
        }

        .chat-btn:hover {
            background-color: #e0e0e0;
        }

        .chat-btn-send {
            background-color: #0066CC;
            color: white;
        }

        .chat-btn-send:hover {
            background-color: #0052a3;
        }

        .chat-btn-send:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }

        .chat-btn-gif {
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .chat-btn-gif:hover {
            background-color: #0066CC;
            color: white;
        }

        .chat-input-box {
            flex: 1;
            display: flex;
            flex-direction: column;
        }

        .chat-input {
            width: 100%;
            border: 2px solid #e0e0e0;
            border-radius: 20px;
            padding: 10px 15px;
            font-size: 14px;
            resize: none;
            font-family: inherit;
            min-height: 40px;
            max-height: 120px;
        }

        .chat-input:focus {
            outline: none;
            border-color: #0066CC;
        }

        /* Upload Progress */
        .chat-upload-progress {
            margin-top: 10px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .progress-bar {
            flex: 1;
            height: 6px;
            background-color: #e0e0e0;
            border-radius: 3px;
            overflow: hidden;
        }

        .progress-fill {
            height: 100%;
            background-color: #0066CC;
            transition: width 0.3s;
            width: 0%;
        }

        .progress-text {
            font-size: 12px;
            color: #666;
            white-space: nowrap;
        }

        /* Reactions */
        .chat-message-reactions {
            display: flex;
            flex-wrap: wrap;
            gap: 5px;
            margin-top: 5px;
        }

        .chat-reaction {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            background-color: #f0f0f0;
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 12px;
            cursor: pointer;
            transition: background-color 0.2s;
        }

        .chat-reaction:hover {
            background-color: #e0e0e0;
        }

        .chat-reaction.own-reaction {
            background-color: #E3F2FD;
        }

        .chat-reaction-emoji {
            font-size: 14px;
        }

        .chat-reaction-count {
            font-weight: 500;
        }

        /* Reply Button */
        .chat-message-actions {
            display: flex;
            gap: 10px;
            margin-top: 5px;
            opacity: 0;
            transition: opacity 0.2s;
        }

        .chat-message:hover .chat-message-actions {
            opacity: 1;
        }

        .chat-action-btn {
            background: none;
            border: none;
            color: #999;
            cursor: pointer;
            font-size: 12px;
            padding: 4px 8px;
            border-radius: 4px;
            transition: all 0.2s;
        }

        .chat-action-btn:hover {
            background-color: #f0f0f0;
            color: #666;
        }

        /* Right Sidebar - Members */
        .family-sidebar {
            width: 280px;
            background-color: white;
            border-radius: 0;
            box-shadow: -2px 0 8px rgba(0,0,0,0.1);
            padding: 20px;
            height: calc(100vh - 80px);
            overflow-y: auto;
            overflow-x: visible; /* Allow tooltips to overflow */
            flex-shrink: 0;
            position: fixed;
            right: 0;
            top: 80px;
            z-index: 100;
            border-left: 1px solid #e0e0e0;
        }

        /* Ensure sidebar container allows overflow */
        .family-sidebar,
        .family-sidebar-left,
        .sidebar-section,
        .member-list,
        .member-item {
            overflow: visible !important;
        }

        .family-main {
            flex: 1;
            min-width: 0;
            margin-left: 310px; /* Left sidebar width + gap */
            margin-right: 310px; /* Right sidebar width + gap */
            padding: 20px;
            max-width: none !important;
            width: calc(100vw - 620px) !important; /* Full viewport width minus both sidebars */
            box-sizing: border-box;
        }

        .sidebar-section {
            margin-bottom: 30px;
            overflow: visible; /* Allow tooltips to overflow */
        }

        .sidebar-section:last-child {
            margin-bottom: 0;
        }

        .member-list {
            overflow: visible; /* Allow tooltips to overflow */
        }

        .sidebar-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e0e0e0;
        }

        .sidebar-title {
            font-size: 12px;
            font-weight: bold;
            color: #666;
            letter-spacing: 1px;
            text-transform: uppercase;
        }

        .sidebar-count {
            font-size: 12px;
            color: #999;
            background-color: #f0f0f0;
            padding: 2px 8px;
            border-radius: 10px;
        }

        .member-list {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .member-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 8px;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.2s;
            position: relative;
        }

        .member-item:hover {
            background-color: #f5f5f5;
        }

        .member-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            overflow: hidden;
            background-color: #e0e0e0;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }

        .member-avatar img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .member-info {
            flex: 1;
            min-width: 0;
        }

        .member-name {
            font-size: 14px;
            font-weight: 500;
            color: #333;
            display: flex;
            align-items: center;
            gap: 8px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .owner-badge {
            font-size: 10px;
            font-weight: bold;
            color: #FF6600;
            background-color: #FFF3E0;
            padding: 2px 6px;
            border-radius: 3px;
            text-transform: uppercase;
        }

        .member-actions {
            display: flex;
            gap: 5px;
            opacity: 0;
            transition: opacity 0.2s;
        }

        .member-item:hover .member-actions {
            opacity: 1;
        }

        .btn-transfer, .btn-kick {
            font-size: 11px;
            padding: 4px 8px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            font-weight: 500;
        }

        .btn-transfer {
            background-color: #0066CC;
            color: white;
        }

        .btn-transfer:hover {
            background-color: #0052a3;
        }

        .btn-kick {
            background-color: #d32f2f;
            color: white;
        }

        .btn-kick:hover {
            background-color: #b71c1c;
        }

        /* Child Tooltip - Display on LEFT side */
        .child-item {
            position: relative;
            overflow: visible; /* Allow tooltip to overflow */
        }

        .child-tooltip {
            position: absolute;
            right: 100%; /* Show on left side of sidebar */
            top: 0;
            margin-right: 10px;
            background-color: #333;
            color: white;
            padding: 12px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 10000; /* High z-index to appear above everything */
            opacity: 0;
            pointer-events: none;
            transition: opacity 0.2s;
            min-width: 180px;
            white-space: nowrap;
        }

        .child-item:hover .child-tooltip {
            opacity: 1;
            pointer-events: auto;
        }

        /* Parent Tooltip - Display on LEFT side */
        .parent-item {
            position: relative;
            overflow: visible; /* Allow tooltip to overflow */
        }

        .parent-tooltip {
            position: absolute;
            right: 100%; /* Show on left side of sidebar */
            top: 0;
            margin-right: 10px;
            background-color: #333;
            color: white;
            padding: 12px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 10000; /* High z-index to appear above everything */
            opacity: 0;
            pointer-events: none;
            transition: opacity 0.2s;
            min-width: 180px;
            white-space: nowrap;
        }

        .parent-item:hover .parent-tooltip {
            opacity: 1;
            pointer-events: auto;
        }

        .tooltip-content {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .tooltip-stat {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .tooltip-label {
            font-size: 12px;
            color: #ccc;
        }

        .tooltip-value {
            font-size: 14px;
            font-weight: bold;
            color: white;
        }

        .tooltip-value.completed {
            color: #4CAF50;
        }

        .tooltip-value.failed {
            color: #f44336;
        }

        /* Code Display */
        .code-display {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-top: 10px;
        }

        .btn-copy-code, .btn-change-code {
            padding: 8px 16px;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.3s;
            background-color: white;
            color: #333;
        }

        .btn-copy-code {
            border-color: #e0e0e0;
        }

        .btn-copy-code:hover {
            background-color: #f5f5f5;
            border-color: #0066CC;
            color: #0066CC;
        }

        .btn-change-code {
            border-color: #e0e0e0;
        }

        .btn-change-code:hover {
            background-color: #f5f5f5;
            border-color: #FF6600;
            color: #FF6600;
        }

        /* Owner Actions */
        .owner-actions-panel {
            margin-top: 30px;
        }

        .btn-leave-family {
            padding: 10px 20px;
            background-color: #d32f2f;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: background-color 0.3s;
        }

        .btn-leave-family:hover {
            background-color: #b71c1c;
        }

        /* Custom Confirmation Modal */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 10000;
            align-items: center;
            justify-content: center;
        }

        .modal-overlay.show {
            display: flex;
        }

        .modal-content {
            background-color: white;
            border-radius: 10px;
            padding: 30px;
            max-width: 500px;
            width: 90%;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
            animation: modalSlideIn 0.3s ease-out;
        }

        @keyframes modalSlideIn {
            from {
                opacity: 0;
                transform: translateY(-20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .modal-header {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
        }

        .modal-icon {
            width: 48px;
            height: 48px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            flex-shrink: 0;
        }

        .modal-icon.warning {
            background-color: #FFF3E0;
            color: #FF6600;
        }

        .modal-icon.danger {
            background-color: #FFEBEE;
            color: #d32f2f;
        }

        .modal-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin: 0;
        }

        .modal-message {
            color: #666;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 25px;
        }

        .modal-actions {
            display: flex;
            gap: 10px;
            justify-content: flex-end;
        }

        .modal-btn {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.3s;
        }

        .modal-btn-cancel {
            background-color: #f5f5f5;
            color: #333;
            border: 2px solid #e0e0e0;
        }

        .modal-btn-cancel:hover {
            background-color: #e0e0e0;
        }

        .modal-btn-confirm {
            background-color: #0066CC;
            color: white;
        }

        .modal-btn-confirm:hover {
            background-color: #0052a3;
        }

        .modal-btn-danger {
            background-color: #d32f2f;
            color: white;
        }

        .modal-btn-danger:hover {
            background-color: #b71c1c;
        }

        /* Enhanced Message Display */
        .message-toast {
            position: fixed;
            top: 20px;
            right: 20px;
            min-width: 300px;
            max-width: 500px;
            padding: 15px 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 10001;
            display: none;
            animation: toastSlideIn 0.3s ease-out;
        }

        .message-toast.show {
            display: block;
        }

        @keyframes toastSlideIn {
            from {
                opacity: 0;
                transform: translateX(100%);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }

        .message-toast.success {
            background-color: #e8f5e9;
            border-left: 4px solid #2e7d32;
            color: #2e7d32;
        }

        .message-toast.error {
            background-color: #ffebee;
            border-left: 4px solid #d32f2f;
            color: #d32f2f;
        }

        .message-toast-content {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
        }

        .message-toast-text {
            flex: 1;
            font-size: 14px;
            font-weight: 500;
        }

        .message-toast-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: inherit;
            opacity: 0.7;
            padding: 0;
            width: 24px;
            height: 24px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: all 0.2s;
        }

        .message-toast-close:hover {
            opacity: 1;
            background-color: rgba(0,0,0,0.1);
        }

        /* Right-Click Context Menu */
        .context-menu {
            position: fixed;
            background-color: white;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            padding: 8px 0;
            min-width: 180px;
            z-index: 10002;
            display: none;
            animation: contextMenuFadeIn 0.2s ease-out;
        }

        .context-menu.show {
            display: block;
        }

        @keyframes contextMenuFadeIn {
            from {
                opacity: 0;
                transform: scale(0.95);
            }
            to {
                opacity: 1;
                transform: scale(1);
            }
        }

        .context-menu-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 10px 16px;
            cursor: pointer;
            transition: background-color 0.2s;
            font-size: 14px;
            color: #333;
        }

        .context-menu-item:hover {
            background-color: #f5f5f5;
        }

        .context-menu-item.danger {
            color: #d32f2f;
        }

        .context-menu-item.danger:hover {
            background-color: #ffebee;
        }

        .context-menu-icon {
            font-size: 16px;
            width: 20px;
            text-align: center;
        }

        .context-menu-text {
            flex: 1;
        }

        /* Enhanced inline messages */
        .error-message, .success-message {
            padding: 15px 20px;
            border-radius: 8px;
            margin: 15px 0;
            display: flex;
            align-items: center;
            gap: 10px;
            animation: messageFadeIn 0.3s ease-out;
        }

        @keyframes messageFadeIn {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .error-message {
            background-color: #ffebee;
            border-left: 4px solid #d32f2f;
            color: #d32f2f;
        }

        .success-message {
            background-color: #e8f5e9;
            border-left: 4px solid #2e7d32;
            color: #2e7d32;
        }

        /* Toast Messages */
        .message-toast {
            position: fixed;
            bottom: 20px;
            right: 20px;
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            padding: 15px 20px;
            min-width: 300px;
            max-width: 500px;
            z-index: 10001;
            transform: translateY(100px);
            opacity: 0;
            transition: all 0.3s ease-out;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
        }

        .message-toast.show {
            transform: translateY(0);
            opacity: 1;
        }

        .message-toast.success {
            border-left: 4px solid #2e7d32;
        }

        .message-toast.error {
            border-left: 4px solid #d32f2f;
        }

        .message-toast.info {
            border-left: 4px solid #0066CC;
        }

        .message-toast-content {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            flex: 1;
        }

        .message-toast-text {
            flex: 1;
            color: #333;
            font-size: 14px;
        }

        .message-toast-close {
            background: none;
            border: none;
            font-size: 20px;
            color: #999;
            cursor: pointer;
            padding: 0;
            width: 24px;
            height: 24px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: all 0.2s;
        }

        .message-toast-close:hover {
            background-color: #f0f0f0;
            color: #666;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .family-layout {
                flex-direction: column;
            }

            .family-sidebar {
                width: 100%;
                height: auto;
                min-height: auto;
                position: relative;
                top: 0;
                right: auto;
                border-left: none;
                border-top: 1px solid #e0e0e0;
            }

            .family-main {
                margin-right: 0;
            }

            .message-toast {
                right: 10px;
                left: 10px;
                max-width: none;
            }

            /* On mobile, tooltips can show on right side if needed */
            .child-tooltip,
            .parent-tooltip {
                right: auto;
                left: 100%;
                margin-right: 0;
                margin-left: 10px;
            }
        }
    </style>
    <script>
        function showTab(tabName) {
            // Hide all forms
            document.getElementById('createForm').classList.add('hidden');
            document.getElementById('joinForm').classList.add('hidden');
            
            // Remove active class from all tabs
            document.querySelectorAll('.tab').forEach(tab => tab.classList.remove('active'));
            
            // Show selected form and activate tab
            if (tabName === 'create') {
                document.getElementById('createForm').classList.remove('hidden');
                document.getElementById('tabCreate').classList.add('active');
            } else {
                document.getElementById('joinForm').classList.remove('hidden');
                document.getElementById('tabJoin').classList.add('active');
            }
        }

        function showLoadingCreate() {
            var btn = document.getElementById('<%= btnCreateFamily.ClientID %>');
            if (btn) {
                setTimeout(function() {
                    if (btn && !btn.disabled) {
                        btn.disabled = true;
                        if (btn.tagName === 'INPUT') {
                            btn.value = 'Creating...';
                        } else {
                            btn.innerHTML = '<span class="spinner"></span>Creating...';
                        }
                    }
                }, 10);
            }
        }

        function showLoadingJoin() {
            var btn = document.getElementById('<%= btnJoinFamily.ClientID %>');
            if (btn) {
                setTimeout(function() {
                    if (btn && !btn.disabled) {
                        btn.disabled = true;
                        if (btn.tagName === 'INPUT') {
                            btn.value = 'Joining...';
                        } else {
                            btn.innerHTML = '<span class="spinner"></span>Joining...';
                        }
                    }
                }, 10);
            }
        }

        // Copy family code to clipboard
        function copyFamilyCode() {
            // Try to get the code from the div wrapper first
            var codeElement = document.getElementById('familyCodeDisplay');
            if (!codeElement) {
                // Fallback: try to get by literal ID
                codeElement = document.getElementById('<%= litFamilyCode.ClientID %>');
            }
            
            if (!codeElement) {
                console.error('Family code element not found');
                return false;
            }

            var code = codeElement.innerText || codeElement.textContent;
            if (!code || code.trim() === '') {
                console.error('Family code is empty');
                return false;
            }
            
            // Trim whitespace
            code = code.trim();

            // Use modern Clipboard API if available, otherwise fallback to execCommand
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(code).then(function() {
                    // Show success toast message
                    showToastMessage('Code copied!', 'success');
                }).catch(function(err) {
                    console.error('Failed to copy:', err);
                    showToastMessage('Failed to copy code. Please copy manually: ' + code, 'error');
                });
                return false;
            } else {
                // Fallback to execCommand for older browsers
                var textarea = document.createElement('textarea');
                textarea.value = code;
                textarea.style.position = 'fixed';
                textarea.style.opacity = '0';
                textarea.style.left = '-9999px';
                document.body.appendChild(textarea);
                textarea.select();
                textarea.setSelectionRange(0, 99999); // For mobile devices

                var textareaRemoved = false;
                try {
                    var successful = document.execCommand('copy');
                    
                    // Safely remove textarea
                    if (textarea.parentNode) {
                        document.body.removeChild(textarea);
                        textareaRemoved = true;
                    }
                    
                    if (successful) {
                        // Show success toast message
                        showToastMessage('Code copied!', 'success');
                        return false;
                    } else {
                        showToastMessage('Failed to copy code. Please copy manually: ' + code, 'error');
                        return false;
                    }
                } catch (err) {
                    // Safely remove textarea if not already removed
                    if (!textareaRemoved && textarea.parentNode) {
                        try {
                            document.body.removeChild(textarea);
                        } catch (removeErr) {
                            // Ignore removal errors
                        }
                    }
                    // Fallback: show code in toast
                    showToastMessage('Copy to clipboard not supported. Family code: ' + code, 'error');
                    return false;
                }
            }
        }

        // Custom Confirmation Modal Functions
        var currentConfirmCallback = null;

        function showConfirmModal(title, message, iconClass, confirmText, confirmClass, callback) {
            var modal = document.getElementById('confirmModal');
            var modalTitle = document.getElementById('modalTitle');
            var modalMessage = document.getElementById('modalMessage');
            var modalIcon = document.getElementById('modalIcon');
            var modalConfirmBtn = document.getElementById('modalConfirmBtn');
            
            if (!modal) return false;
            
            modalTitle.textContent = title;
            modalMessage.textContent = message;
            modalIcon.className = 'modal-icon ' + iconClass;
            modalConfirmBtn.textContent = confirmText;
            modalConfirmBtn.className = 'modal-btn modal-btn-confirm ' + confirmClass;
            
            currentConfirmCallback = callback;
            modal.classList.add('show');
            
            return false;
        }

        function closeConfirmModal() {
            var modal = document.getElementById('confirmModal');
            if (modal) {
                modal.classList.remove('show');
            }
            currentConfirmCallback = null;
        }

        function confirmModalAction() {
            if (currentConfirmCallback) {
                currentConfirmCallback();
            }
            closeConfirmModal();
        }

        // Helper function to trigger postback safely
        function triggerPostBack(uniqueId, buttonId) {
            // Try __doPostBack first (ASP.NET standard)
            if (typeof __doPostBack !== 'undefined') {
                __doPostBack(uniqueId, '');
                return;
            }
            
            // Fallback: Find button and trigger click without event prevention
            var btn = document.getElementById(buttonId);
            if (btn) {
                // Temporarily remove event listeners and OnClientClick behavior
                var originalOnClick = btn.onclick;
                btn.onclick = null;
                
                // Create a new click event that will actually submit
                var clickEvent = document.createEvent('MouseEvents');
                clickEvent.initEvent('click', true, true);
                
                // Submit the form with the button's name
                var form = document.getElementById('form1');
                if (form) {
                    // Create a hidden input with the button's unique ID as the name
                    var hiddenInput = document.createElement('input');
                    hiddenInput.type = 'hidden';
                    hiddenInput.name = uniqueId;
                    hiddenInput.value = '';
                    form.appendChild(hiddenInput);
                    
                    // Submit the form
                    form.submit();
                } else {
                    // Last resort: try direct click
                    btn.click();
                }
            }
        }

        // Confirmation for changing family code
        function confirmChangeCode() {
            return showConfirmModal(
                'Change Family Code',
                'Are you sure you want to change the family code? The old code will no longer work. All children will need to use the new code to join.',
                'warning',
                'Change Code',
                'modal-btn-confirm',
                function() {
                    console.log('Modal confirmed, triggering postback...');
                    // Use the hidden button to trigger postback
                    var hiddenBtn = document.getElementById('<%= btnChangeCodeHidden.ClientID %>');
                    if (hiddenBtn) {
                        console.log('Clicking hidden button to trigger postback...');
                        hiddenBtn.click();
                    } else {
                        console.error('Hidden button not found!');
                        // Fallback: try __doPostBack
                        var uniqueId = '<%= btnChangeCodeHidden.UniqueID %>';
                        if (typeof __doPostBack !== 'undefined') {
                            console.log('Using __doPostBack as fallback...');
                            __doPostBack(uniqueId, '');
                        } else {
                            console.error('__doPostBack not available!');
                            alert('Unable to change code. Please refresh the page and try again.');
                        }
                    }
                }
            );
        }

        // Confirmation for leaving family
        function confirmLeaveFamily() {
            return showConfirmModal(
                'Leave Family',
                'Are you sure you want to leave this family? If you are the owner and there are no children, ownership will be transferred to another parent.',
                'danger',
                'Leave Family',
                'modal-btn-danger',
                function() {
                    // Trigger postback using form submission
                    var btnId = '<%= btnLeaveFamily != null ? btnLeaveFamily.ClientID : "" %>';
                    var uniqueId = '<%= btnLeaveFamily != null ? btnLeaveFamily.UniqueID : "" %>';
                    
                    if (!btnId || !uniqueId) {
                        console.error('btnLeaveFamily not available');
                        return;
                    }
                    
                    var btn = document.getElementById(btnId);
                    
                    if (btn && typeof __doPostBack !== 'undefined') {
                        // Use ASP.NET's __doPostBack
                        __doPostBack(uniqueId, '');
                    } else {
                        // Fallback: submit form with button's event target
                        var form = document.getElementById('form1');
                        if (form && btn) {
                            // Create hidden input to simulate button click
                            var hiddenInput = document.createElement('input');
                            hiddenInput.type = 'hidden';
                            hiddenInput.name = '__EVENTTARGET';
                            hiddenInput.value = uniqueId;
                            form.appendChild(hiddenInput);
                            
                            var hiddenInput2 = document.createElement('input');
                            hiddenInput2.type = 'hidden';
                            hiddenInput2.name = '__EVENTARGUMENT';
                            hiddenInput2.value = '';
                            form.appendChild(hiddenInput2);
                            
                            // Submit the form
                            form.submit();
                        } else {
                            // Last resort: try direct click
                            if (btn) btn.click();
                        }
                    }
                }
            );
        }

        // Show toast message
        function showToastMessage(message, type) {
            var toast = document.getElementById('messageToast');
            if (!toast) return;
            
            toast.className = 'message-toast ' + type;
            toast.querySelector('.message-toast-text').textContent = message;
            
            // Force compact sizing
            toast.style.height = 'auto';
            toast.style.maxHeight = '45px';
            toast.style.minHeight = 'auto';
            toast.style.width = 'auto';
            toast.style.maxWidth = '220px';
            toast.style.padding = '8px 12px';
            toast.style.display = 'inline-flex';
            toast.style.flexDirection = 'row';
            toast.style.alignItems = 'center';
            
            toast.classList.add('show');
            
            // Auto-dismiss after 5 seconds
            setTimeout(function() {
                toast.classList.remove('show');
            }, 5000);
        }

        // Close toast message
        function closeToast() {
            var toast = document.getElementById('messageToast');
            if (toast) {
                toast.classList.remove('show');
            }
        }

        // Right-Click Context Menu Functions
        var contextMenuData = {
            userId: null,
            userType: null,
            userName: null,
            isOwner: false
        };

        function showContextMenu(event, userType, userId, userName, isOwner) {
            event.preventDefault();
            event.stopPropagation();

            // Check permissions - get from data attributes
            var layout = document.querySelector('.family-layout');
            var currentUserRole = layout ? layout.getAttribute('data-current-user-role') : '';
            var currentUserIsOwner = layout ? layout.getAttribute('data-current-user-is-owner') === 'True' : false;

            // For children: parents can kick
            if (userType === 'child') {
                if (currentUserRole !== 'PARENT') {
                    return; // Only parents can kick children
                }
            }
            // For parents: only owner can kick
            else if (userType === 'parent') {
                if (!currentUserIsOwner || isOwner) {
                    return; // Only owner can kick parents, and can't kick themselves
                }
            }

            // Store context menu data
            contextMenuData.userId = userId;
            contextMenuData.userType = userType;
            contextMenuData.userName = userName;
            contextMenuData.isOwner = isOwner || false;

            // Show context menu
            var contextMenu = document.getElementById('contextMenu');
            if (contextMenu) {
                contextMenu.style.display = 'block';
                contextMenu.style.left = event.pageX + 'px';
                contextMenu.style.top = event.pageY + 'px';
                contextMenu.classList.add('show');

                // Add danger class for kick action
                var kickItem = document.getElementById('contextMenuKick');
                if (kickItem) {
                    kickItem.classList.add('danger');
                }
            }

            // Close context menu when clicking elsewhere
            setTimeout(function() {
                document.addEventListener('click', closeContextMenu);
                document.addEventListener('contextmenu', closeContextMenu);
            }, 10);
        }

        function closeContextMenu() {
            var contextMenu = document.getElementById('contextMenu');
            if (contextMenu) {
                contextMenu.style.display = 'none';
                contextMenu.classList.remove('show');
            }
            document.removeEventListener('click', closeContextMenu);
            document.removeEventListener('contextmenu', closeContextMenu);
        }

        function contextMenuKick() {
            closeContextMenu();

            var userName = contextMenuData.userName || 'this user';
            var userType = contextMenuData.userType;
            var isOwner = contextMenuData.isOwner;

            var title = 'Kick Out Member';
            var message = 'Are you sure you want to remove ' + userName + ' from the family?';
            if (userType === 'child') {
                message += ' Their points will be reset.';
            } else if (userType === 'parent') {
                message += ' This action cannot be undone.';
            }

            // Show confirmation modal
            showConfirmModal(
                title,
                message,
                'danger',
                'Kick Out',
                'modal-btn-danger',
                function() {
                    // Set hidden fields and trigger postback
                    var hidUserId = document.getElementById('<%= hidKickUserId.ClientID %>');
                    var hidUserType = document.getElementById('<%= hidKickUserType.ClientID %>');
                    var btnKick = document.getElementById('<%= btnKickChildHidden.ClientID %>');

                    if (hidUserId && hidUserType && btnKick) {
                        hidUserId.value = contextMenuData.userId;
                        hidUserType.value = contextMenuData.userType;
                        btnKick.click();
                    }
                }
            );
        }

        // Initialize button click handlers
        window.addEventListener('load', function() {
            // Copy button handler
            var copyBtn = document.getElementById('<%= btnCopyCode.ClientID %>');
            if (copyBtn) {
                copyBtn.addEventListener('click', function(e) {
                    e.preventDefault();
                    copyFamilyCode();
                });
            }
            
            // Change code button handler - show modal instead of direct click
            var changeBtn = document.getElementById('<%= btnChangeCode.ClientID %>');
            if (changeBtn) {
                changeBtn.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('Change code button clicked, showing modal...');
                    confirmChangeCode();
                });
            }

            // Close context menu on outside click
            document.addEventListener('click', function(e) {
                var contextMenu = document.getElementById('contextMenu');
                if (contextMenu && !contextMenu.contains(e.target)) {
                    closeContextMenu();
                }
            });

            // Initialize chat
            initializeChat();
        });

        // ============================================
        // CHAT FUNCTIONALITY
        // ============================================
        var chatState = {
            familyId: null,
            userId: null,
            lastMessageId: 0,
            pollingInterval: null,
            replyToMessageId: null,
            replyToMessageText: null,
            replyToUserName: null
        };

        function initializeChat() {
            // Get family ID and user ID from server (will be set by code-behind)
            var familyIdElement = document.getElementById('hidFamilyId');
            var userIdElement = document.getElementById('hidUserId');
            
            if (familyIdElement && userIdElement) {
                chatState.familyId = parseInt(familyIdElement.value);
                chatState.userId = parseInt(userIdElement.value);
                
                // Load initial messages
                loadMessages();
                
                // Start polling for new messages (every 5 seconds)
                chatState.pollingInterval = setInterval(function() {
                    checkForNewMessages();
                }, 5000);
                
                // Setup event handlers
                setupChatHandlers();
            } else {
                console.error('Chat initialization failed: Family ID or User ID not found');
            }
        }

        function setupChatHandlers() {
            // Send message button
            var btnSend = document.getElementById('btnSendMessage');
            if (btnSend) {
                btnSend.addEventListener('click', sendMessage);
            }

            // Enter key to send (Shift+Enter for new line)
            var txtMessage = document.getElementById('txtMessage');
            if (txtMessage) {
                txtMessage.addEventListener('keydown', function(e) {
                    if (e.key === 'Enter' && !e.shiftKey) {
                        e.preventDefault();
                        sendMessage();
                    }
                });
                
                // Auto-resize textarea
                txtMessage.addEventListener('input', function() {
                    this.style.height = 'auto';
                    this.style.height = Math.min(this.scrollHeight, 120) + 'px';
                });
            }

            // Upload image button
            var btnUpload = document.getElementById('btnUploadImage');
            var fileUpload = document.getElementById('fileUploadImage');
            if (btnUpload && fileUpload) {
                btnUpload.addEventListener('click', function() {
                    fileUpload.click();
                });
                fileUpload.addEventListener('change', function() {
                    if (this.files && this.files.length > 0) {
                        uploadImage(this.files[0]);
                    }
                });
            }

            // GIF button
            var btnGIF = document.getElementById('btnGIF');
            if (btnGIF) {
                btnGIF.addEventListener('click', function() {
                    showGIFPicker();
                });
            }

            // Cancel reply button
            var btnCancelReply = document.getElementById('btnCancelReply');
            if (btnCancelReply) {
                btnCancelReply.addEventListener('click', cancelReply);
            }
        }

        function loadMessages() {
            var chatMessages = document.getElementById('chatMessages');
            var chatLoading = document.getElementById('chatLoading');
            var chatEmpty = document.getElementById('chatEmpty');
            
            if (!chatMessages) return;

            // Show loading
            if (chatLoading) chatLoading.style.display = 'block';
            if (chatEmpty) chatEmpty.style.display = 'none';

            // Call server to get messages
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/GetChatMessages', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            if (response.d && response.d.success) {
                                var messages = response.d.messages || [];
                                
                                // Clear existing messages
                                chatMessages.innerHTML = '';
                                
                                if (messages.length === 0) {
                                    if (chatLoading) chatLoading.style.display = 'none';
                                    if (chatEmpty) chatEmpty.style.display = 'block';
                                } else {
                                    if (chatLoading) chatLoading.style.display = 'none';
                                    if (chatEmpty) chatEmpty.style.display = 'none';
                                    
                                    // Display messages (oldest first, like Facebook Messenger)
                                    messages.forEach(function(msg) {
                                        appendMessage(msg, false, chatMessages); // false = not a new message
                                    });
                                    
                                    // Update last message ID (newest message is last)
                                    if (messages.length > 0) {
                                        chatState.lastMessageId = messages[messages.length - 1].Id;
                                    }
                                    
                                    // Scroll to bottom to show newest messages
                                    scrollToBottom();
                                }
                            } else {
                                console.error('Failed to load messages:', response.d.error);
                                if (chatLoading) chatLoading.style.display = 'none';
                            }
                        } catch (e) {
                            console.error('Error parsing messages response:', e);
                            if (chatLoading) chatLoading.style.display = 'none';
                        }
                    } else {
                        console.error('Failed to load messages. Status:', xhr.status);
                        if (chatLoading) chatLoading.style.display = 'none';
                    }
                }
            };
            xhr.send(JSON.stringify({ familyId: chatState.familyId, userId: chatState.userId }));
        }

        function checkForNewMessages() {
            if (!chatState.familyId || !chatState.userId || chatState.lastMessageId === 0) return;

            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/GetNewChatMessages', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response.d && response.d.success) {
                            var messages = response.d.messages || [];
                            if (messages.length > 0) {
                                // Append new messages at the bottom (oldest first, like Facebook Messenger)
                                var chatMessages = document.getElementById('chatMessages');
                                var wasAtBottom = isScrolledToBottom(chatMessages);
                                
                                messages.forEach(function(msg) {
                                    appendMessage(msg, true, chatMessages); // true = new message, append
                                    chatState.lastMessageId = Math.max(chatState.lastMessageId, msg.Id);
                                });
                                
                                // Auto-scroll to bottom only if user was already at bottom
                                if (wasAtBottom) {
                                    setTimeout(function() {
                                        scrollToBottom();
                                    }, 100);
                                }
                            }
                            
                            // Also refresh reactions for all visible messages
                            refreshAllReactions();
                        }
                    } catch (e) {
                        console.error('Error checking for new messages:', e);
                    }
                }
            };
            xhr.send(JSON.stringify({ 
                familyId: chatState.familyId, 
                userId: chatState.userId, 
                lastMessageId: chatState.lastMessageId 
            }));
        }
        
        function refreshAllReactions() {
            // Get all message elements in the chat
            var chatMessages = document.getElementById('chatMessages');
            if (!chatMessages) return;
            
            // Find all reaction containers (they have id like "reactions-123")
            var reactionContainers = chatMessages.querySelectorAll('[id^="reactions-"]');
            
            reactionContainers.forEach(function(container) {
                // Extract message ID from the container ID (e.g., "reactions-123" -> 123)
                var messageId = parseInt(container.id.replace('reactions-', ''));
                if (!isNaN(messageId)) {
                    // Reload reactions for this message
                    loadReactions(messageId);
                }
            });
        }

        function sendMessage() {
            var txtMessage = document.getElementById('txtMessage');
            if (!txtMessage) return;

            var messageText = txtMessage.value.trim();
            if (!messageText && !chatState.replyToMessageId) return;

            // Disable send button
            var btnSend = document.getElementById('btnSendMessage');
            if (btnSend) btnSend.disabled = true;

            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/SendChatMessage', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    if (btnSend) btnSend.disabled = false;
                    
                    if (xhr.status === 200) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            if (response.d && response.d.success) {
                                // Clear input
                                txtMessage.value = '';
                                txtMessage.style.height = 'auto';
                                
                                // Cancel reply if active
                                cancelReply();
                                
                                // Reload messages to show the new one
                                loadMessages();
                                
                                // Show success indicator
                                showMessageStatus('sent');
                            } else {
                                showMessageStatus('failed');
                                console.error('Failed to send message:', response.d.error);
                            }
                        } catch (e) {
                            showMessageStatus('failed');
                            console.error('Error parsing send response:', e);
                        }
                    } else {
                        showMessageStatus('failed');
                        console.error('Failed to send message. Status:', xhr.status);
                    }
                }
            };
            xhr.send(JSON.stringify({ 
                familyId: chatState.familyId, 
                userId: chatState.userId, 
                messageText: messageText,
                replyToMessageId: chatState.replyToMessageId || null
            }));
        }

        function uploadImage(file) {
            if (!file) return;

            // Validate file type
            if (!file.type.match('image.*')) {
                showToastMessage('Please select an image file.', 'error');
                return;
            }

            // Validate file size (50MB)
            if (file.size > 50 * 1024 * 1024) {
                showToastMessage('File size must be less than 50MB.', 'error');
                return;
            }

            // Show progress bar
            var uploadProgress = document.getElementById('uploadProgress');
            var progressFill = document.getElementById('progressFill');
            var progressText = document.getElementById('progressText');
            if (uploadProgress) uploadProgress.style.display = 'flex';
            if (progressText) progressText.textContent = 'Uploading...';

            // Create FormData
            var formData = new FormData();
            formData.append('file', file);

            // Upload via ASHX handler
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'ChatImageUpload.ashx', true);
            
            // Track upload progress
            xhr.upload.addEventListener('progress', function(e) {
                if (e.lengthComputable && progressFill) {
                    var percentComplete = (e.loaded / e.total) * 100;
                    progressFill.style.width = percentComplete + '%';
                    if (progressText) {
                        progressText.textContent = 'Uploading... ' + Math.round(percentComplete) + '%';
                    }
                }
            });

            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    if (uploadProgress) uploadProgress.style.display = 'none';
                    
                    if (xhr.status === 200) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            if (response.success) {
                                // Send image message
                                sendImageMessage(response.imageUrl);
                            } else {
                                showToastMessage('Upload failed: ' + (response.error || 'Unknown error'), 'error');
                            }
                        } catch (e) {
                            showToastMessage('Error processing upload response.', 'error');
                            console.error('Error parsing upload response:', e);
                        }
                    } else {
                        showToastMessage('Upload failed. Please try again.', 'error');
                    }
                }
            };

            xhr.send(formData);
        }

        function sendImageMessage(imageUrl) {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/SendChatImage', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response.d && response.d.success) {
                            loadMessages();
                            showMessageStatus('sent');
                        } else {
                            showMessageStatus('failed');
                        }
                    } catch (e) {
                        console.error('Error sending image message:', e);
                        showMessageStatus('failed');
                    }
                }
            };
            xhr.send(JSON.stringify({ 
                familyId: chatState.familyId, 
                userId: chatState.userId, 
                imageUrl: imageUrl 
            }));
        }

        function scrollToBottom() {
            var chatMessages = document.getElementById('chatMessages');
            if (chatMessages) {
                chatMessages.scrollTop = chatMessages.scrollHeight;
            }
        }

        function isScrolledToBottom(element) {
            if (!element) return false;
            var threshold = 100; // Consider "at bottom" if within 100px
            return element.scrollHeight - element.scrollTop - element.clientHeight < threshold;
        }

        function appendMessage(msg, isNewMessage, chatContainer) {
            var chatMessages = chatContainer || document.getElementById('chatMessages');
            if (!chatMessages) return;

            var messageDiv = document.createElement('div');
            messageDiv.className = 'chat-message';
            messageDiv.setAttribute('data-message-id', msg.Id);
            
            // Add animation class for new messages
            if (isNewMessage) {
                messageDiv.classList.add('message-enter');
                if (msg.MessageType === 'System') {
                    messageDiv.classList.add('system-message-enter');
                }
            }

            // Check if it's own message or system message
            if (msg.UserId === chatState.userId) {
                messageDiv.classList.add('own-message');
            } else if (msg.MessageType === 'System' || msg.UserId === null || msg.UserId === undefined || msg.UserId === 0) {
                messageDiv.classList.add('system-message');
                // Add specific class for reward messages
                if (msg.SystemEventType === 'RewardAdded') {
                    messageDiv.classList.add('reward-message');
                }
            }

            // Build message HTML
            var html = '';
            
            // Avatar (skip for system messages)
            if (msg.MessageType !== 'System' && msg.UserId !== null && msg.UserId !== undefined && msg.UserId !== 0) {
                html += '<div class="chat-message-avatar">';
                if (msg.ProfilePicture) {
                    html += '<img src="' + escapeHtml(msg.ProfilePicture) + '" alt="' + escapeHtml((msg.FirstName || '') + ' ' + (msg.LastName || '')) + '" />';
                } else {
                    var initials = (msg.FirstName ? msg.FirstName[0] : '') + (msg.LastName ? msg.LastName[0] : '');
                    html += '<div style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; background-color: #0066CC; color: white; font-weight: bold;">' + (initials || 'U') + '</div>';
                }
                html += '</div>';
            }

            html += '<div class="chat-message-content">';
            
            // Header (name and time) - only for non-system messages
            if (msg.MessageType !== 'System' && msg.UserId !== null && msg.UserId !== undefined && msg.UserId !== 0) {
                html += '<div class="chat-message-header">';
                html += '<span class="chat-message-name">' + escapeHtml((msg.FirstName || '') + ' ' + (msg.LastName || '')) + '</span>';
                html += '<span class="chat-message-time">' + formatTime(msg.CreatedDate) + '</span>';
                html += '</div>';
            }

            // Message bubble
            html += '<div class="chat-message-bubble">';
            
            // Reply preview if this is a reply
            if (msg.ReplyToMessageId && msg.ReplyToMessageText) {
                html += '<div class="chat-reply-preview">';
                html += '<div class="reply-preview-header">';
                html += '<span class="reply-preview-name">' + escapeHtml(msg.ReplyToFirstName + ' ' + msg.ReplyToLastName) + '</span>';
                html += '</div>';
                html += '<div class="reply-preview-text">' + escapeHtml(msg.ReplyToMessageText) + '</div>';
                html += '</div>';
            }

            // Message content based on type
            if (msg.MessageType === 'Text') {
                html += '<div class="chat-message-text">' + escapeHtml(msg.MessageText) + '</div>';
            } else if (msg.MessageType === 'Image') {
                html += '<img src="' + escapeHtml(msg.ImagePath) + '" class="chat-message-image" onclick="openImageModal(\'' + escapeHtml(msg.ImagePath) + '\')" />';
            } else if (msg.MessageType === 'GIF') {
                html += '<img src="' + escapeHtml(msg.GIFUrl) + '" class="chat-message-gif" />';
            } else if (msg.MessageType === 'System') {
                // Add emoji based on system event type
                var emoji = '';
                if (msg.SystemEventType === 'RewardAdded') {
                    emoji = String.fromCharCode(0xD83C, 0xDF89); // 🎉 (surrogate pair)
                } else if (msg.SystemEventType === 'TaskCompleted') {
                    emoji = String.fromCharCode(0x2705); // ✅
                } else if (msg.SystemEventType === 'TaskFailed') {
                    emoji = String.fromCharCode(0x274C); // ❌
                }
                html += '<div class="chat-message-text">' + (emoji ? '<span class="system-emoji">' + emoji + '</span> ' : '') + escapeHtml(msg.MessageText) + '</div>';
            }

            html += '</div>'; // End bubble

            // Reactions (will be loaded separately)
            html += '<div class="chat-message-reactions" id="reactions-' + msg.Id + '"></div>';

            // Actions (reply and react buttons) - only for non-system messages
            if (msg.MessageType !== 'System' && msg.UserId !== null && msg.UserId !== undefined && msg.UserId !== 0) {
                html += '<div class="chat-message-actions">';
                if (msg.UserId !== chatState.userId) {
                    html += '<button type="button" class="chat-action-btn" onclick="replyToMessage(' + msg.Id + ', \'' + escapeHtml((msg.FirstName || '') + ' ' + (msg.LastName || '')) + '\', \'' + escapeHtml(msg.MessageText || '') + '\'); return false;">Reply</button>';
                }
                html += '<button type="button" class="chat-action-btn" onclick="showReactionPicker(' + msg.Id + '); return false;">React</button>';
                html += '</div>';
            }

            html += '</div>'; // End content

            messageDiv.innerHTML = html;
            
            // Always append at bottom (like Facebook Messenger)
            chatMessages.appendChild(messageDiv);
            
            // Load reactions for this message
            loadReactions(msg.Id);
        }

        function loadReactions(messageId) {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/GetMessageReactions', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response.d && response.d.success) {
                            displayReactions(messageId, response.d.reactions);
                        }
                    } catch (e) {
                        console.error('Error loading reactions:', e);
                    }
                }
            };
            xhr.send(JSON.stringify({ messageId: messageId, userId: chatState.userId }));
        }

        function displayReactions(messageId, reactions) {
            var reactionsContainer = document.getElementById('reactions-' + messageId);
            if (!reactionsContainer) return;

            // Group reactions by type
            var reactionGroups = {};
            reactions.forEach(function(reaction) {
                var type = reaction.ReactionType;
                if (!reactionGroups[type]) {
                    reactionGroups[type] = {
                        count: 0,
                        users: [],
                        hasCurrentUser: false
                    };
                }
                reactionGroups[type].count++;
                reactionGroups[type].users.push(reaction.FirstName + ' ' + reaction.LastName);
                if (reaction.UserId === chatState.userId) {
                    reactionGroups[type].hasCurrentUser = true;
                }
            });

            // Build reaction HTML
            var html = '';
            // Use String.fromCharCode for emojis to avoid encoding issues
            var reactionEmojis = {
                'Like': String.fromCharCode(0xD83D, 0xDC4D),      // 👍
                'Love': String.fromCharCode(0x2764, 0xFE0F),      // ❤️
                'Haha': String.fromCharCode(0xD83D, 0xDE02),      // 😂
                'Sad': String.fromCharCode(0xD83D, 0xDE22),       // 😢
                'Angry': String.fromCharCode(0xD83D, 0xDE20)      // 😠
            };

            Object.keys(reactionGroups).forEach(function(type) {
                var group = reactionGroups[type];
                var emoji = reactionEmojis[type] || type;
                var className = 'chat-reaction';
                if (group.hasCurrentUser) {
                    className += ' own-reaction';
                }
                
                html += '<div class="' + className + '" onclick="toggleReaction(' + messageId + ', \'' + type + '\'); if(event) { event.preventDefault(); event.stopPropagation(); } return false;">';
                html += '<span class="chat-reaction-emoji">' + emoji + '</span>';
                html += '<span class="chat-reaction-count">' + group.count + '</span>';
                html += '</div>';
            });

            reactionsContainer.innerHTML = html;
        }

        function showReactionPicker(messageId) {
            // Prevent any form submission or page refresh
            if (event) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            // Create reaction picker modal
            var modal = document.createElement('div');
            modal.id = 'reactionPickerModal';
            modal.style.cssText = 'position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; display: flex; align-items: center; justify-content: center;';
            modal.onclick = function(e) {
                if (e.target === modal) {
                    closeReactionPicker();
                }
            };
            
            var container = document.createElement('div');
            container.style.cssText = 'background: white; border-radius: 12px; padding: 20px; box-shadow: 0 4px 20px rgba(0,0,0,0.3); display: flex; gap: 15px; align-items: center;';
            container.onclick = function(e) { e.stopPropagation(); };
            
            // Reaction types and emojis
            var reactions = [
                { type: 'Like', emoji: String.fromCharCode(0xD83D, 0xDC4D), label: 'Like' },
                { type: 'Love', emoji: String.fromCharCode(0x2764, 0xFE0F), label: 'Love' },
                { type: 'Haha', emoji: String.fromCharCode(0xD83D, 0xDE02), label: 'Haha' },
                { type: 'Sad', emoji: String.fromCharCode(0xD83D, 0xDE22), label: 'Sad' },
                { type: 'Angry', emoji: String.fromCharCode(0xD83D, 0xDE20), label: 'Angry' }
            ];
            
            reactions.forEach(function(reaction) {
                var reactionBtn = document.createElement('button');
                reactionBtn.type = 'button';
                reactionBtn.style.cssText = 'background: transparent; border: 2px solid #e0e0e0; border-radius: 50%; width: 50px; height: 50px; font-size: 24px; cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s; padding: 0;';
                reactionBtn.innerHTML = reaction.emoji;
                reactionBtn.title = reaction.label;
                
                reactionBtn.onmouseover = function() {
                    this.style.borderColor = '#0066CC';
                    this.style.transform = 'scale(1.2)';
                    this.style.backgroundColor = '#f0f7ff';
                };
                reactionBtn.onmouseout = function() {
                    this.style.borderColor = '#e0e0e0';
                    this.style.transform = 'scale(1)';
                    this.style.backgroundColor = 'transparent';
                };
                reactionBtn.onclick = function() {
                    toggleReaction(messageId, reaction.type);
                    closeReactionPicker();
                };
                
                container.appendChild(reactionBtn);
            });
            
            modal.appendChild(container);
            document.body.appendChild(modal);
            
            // Store modal reference
            window.currentReactionModal = modal;
            
            return false;
        }
        
        function closeReactionPicker() {
            var modal = document.getElementById('reactionPickerModal');
            if (modal) {
                document.body.removeChild(modal);
            }
            if (window.currentReactionModal) {
                try {
                    document.body.removeChild(window.currentReactionModal);
                } catch(e) {}
                window.currentReactionModal = null;
            }
        }

        function toggleReaction(messageId, reactionType) {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/ToggleReaction', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response.d && response.d.success) {
                            // Reload reactions for this message
                            loadReactions(messageId);
                            // Also refresh all reactions to ensure other users see the update
                            setTimeout(function() {
                                refreshAllReactions();
                            }, 500);
                        }
                    } catch (e) {
                        console.error('Error toggling reaction:', e);
                    }
                }
            };
            xhr.send(JSON.stringify({ messageId: messageId, userId: chatState.userId, reactionType: reactionType }));
        }

        function replyToMessage(messageId, userName, messageText) {
            chatState.replyToMessageId = messageId;
            chatState.replyToUserName = userName;
            chatState.replyToMessageText = messageText;

            // Show reply preview
            var replyPreview = document.getElementById('replyPreview');
            var replyPreviewName = document.getElementById('replyPreviewName');
            var replyPreviewText = document.getElementById('replyPreviewText');
            
            if (replyPreview && replyPreviewName && replyPreviewText) {
                replyPreviewName.textContent = userName;
                replyPreviewText.textContent = messageText || '[Image]';
                replyPreview.style.display = 'block';
            }

            // Focus on input
            var txtMessage = document.getElementById('txtMessage');
            if (txtMessage) txtMessage.focus();
        }

        function cancelReply() {
            chatState.replyToMessageId = null;
            chatState.replyToUserName = null;
            chatState.replyToMessageText = null;

            var replyPreview = document.getElementById('replyPreview');
            if (replyPreview) replyPreview.style.display = 'none';
        }

        function scrollToBottom() {
            var chatMessages = document.getElementById('chatMessages');
            if (chatMessages) {
                chatMessages.scrollTop = chatMessages.scrollHeight;
            }
        }

        function formatTime(dateString) {
            var date = new Date(dateString);
            var now = new Date();
            var diff = now - date;
            var seconds = Math.floor(diff / 1000);
            var minutes = Math.floor(seconds / 60);
            var hours = Math.floor(minutes / 60);
            var days = Math.floor(hours / 24);

            if (seconds < 60) return 'Just now';
            if (minutes < 60) return minutes + 'm ago';
            if (hours < 24) return hours + 'h ago';
            if (days < 7) return days + 'd ago';
            
            return date.toLocaleDateString();
        }

        function escapeHtml(text) {
            if (!text) return '';
            var div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }

        function showMessageStatus(status) {
            // Could add visual feedback here (e.g., checkmark or X icon)
            // For now, just log
            console.log('Message status:', status);
        }

        function openImageModal(imageUrl) {
            // Simple image modal - could be enhanced
            var modal = document.createElement('div');
            modal.style.cssText = 'position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.9); z-index: 10000; display: flex; align-items: center; justify-content: center; cursor: pointer;';
            modal.onclick = function() { document.body.removeChild(modal); };
            
            var img = document.createElement('img');
            img.src = imageUrl;
            img.style.cssText = 'max-width: 90%; max-height: 90%; border-radius: 8px;';
            img.onclick = function(e) { e.stopPropagation(); };
            
            modal.appendChild(img);
            document.body.appendChild(modal);
        }

        function showGIFPicker() {
            // Create GIF picker modal
            var modal = document.createElement('div');
            modal.id = 'gifPickerModal';
            modal.style.cssText = 'position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.8); z-index: 10000; display: flex; flex-direction: column; align-items: center; padding: 20px; overflow-y: auto;';
            modal.onclick = function(e) {
                if (e.target === modal) {
                    closeGIFPicker();
                }
            };

            var container = document.createElement('div');
            container.style.cssText = 'max-width: 900px; width: 100%; background: white; border-radius: 12px; padding: 25px; max-height: 85vh; overflow-y: auto; box-shadow: 0 4px 20px rgba(0,0,0,0.3);';
            container.onclick = function(e) { e.stopPropagation(); };

            // Header with search
            var header = document.createElement('div');
            header.style.cssText = 'margin-bottom: 20px; border-bottom: 2px solid #e0e0e0; padding-bottom: 15px;';
            
            var title = document.createElement('h3');
            title.textContent = 'Search GIFs';
            title.style.cssText = 'margin: 0 0 15px 0; color: #333; font-size: 24px;';
            header.appendChild(title);

            var searchContainer = document.createElement('div');
            searchContainer.style.cssText = 'display: flex; gap: 10px;';
            
            var searchInput = document.createElement('input');
            searchInput.type = 'text';
            searchInput.placeholder = 'Search for GIFs...';
            searchInput.id = 'gifSearchInput';
            searchInput.style.cssText = 'flex: 1; padding: 12px 15px; border: 2px solid #e0e0e0; border-radius: 8px; font-size: 16px; outline: none;';
            searchInput.addEventListener('keypress', function(e) {
                if (e.key === 'Enter') {
                    searchGIFs();
                }
            });
            
            var searchBtn = document.createElement('button');
            searchBtn.textContent = 'Search';
            searchBtn.style.cssText = 'padding: 12px 24px; background: #0066CC; color: white; border: none; border-radius: 8px; cursor: pointer; font-size: 16px; font-weight: 500;';
            searchBtn.onclick = searchGIFs;
            
            var closeBtn = document.createElement('button');
            closeBtn.innerHTML = '&times;';
            closeBtn.style.cssText = 'padding: 12px 20px; background: #f5f5f5; color: #333; border: none; border-radius: 8px; cursor: pointer; font-size: 24px; line-height: 1;';
            closeBtn.onclick = closeGIFPicker;
            
            searchContainer.appendChild(searchInput);
            searchContainer.appendChild(searchBtn);
            searchContainer.appendChild(closeBtn);
            header.appendChild(searchContainer);
            
            container.appendChild(header);

            // GIF grid container
            var gridContainer = document.createElement('div');
            gridContainer.id = 'gifGridContainer';
            gridContainer.style.cssText = 'min-height: 200px;';
            
            var loadingDiv = document.createElement('div');
            loadingDiv.id = 'gifLoading';
            loadingDiv.textContent = 'Search for GIFs to get started...';
            loadingDiv.style.cssText = 'text-align: center; padding: 40px; color: #999; font-size: 16px;';
            gridContainer.appendChild(loadingDiv);
            
            container.appendChild(gridContainer);
            modal.appendChild(container);
            document.body.appendChild(modal);
            
            // Focus on search input
            setTimeout(function() {
                searchInput.focus();
            }, 100);
            
            // Store modal reference for closing
            window.currentGIFModal = modal;
            
            function searchGIFs() {
                var searchTerm = searchInput.value.trim();
                if (!searchTerm) {
                    showToastMessage('Please enter a search term', 'error');
                    return;
                }
                
                // Show loading
                gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #999;">Searching for GIFs...</div>';
                
                // Get Giphy API key from hidden field
                var giphyApiKeyElement = document.getElementById('<%= hidGiphyApiKey.ClientID %>');
                var giphyApiKey = giphyApiKeyElement ? giphyApiKeyElement.value : '';
                
                if (!giphyApiKey || giphyApiKey.trim() === '') {
                    gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #d32f2f;">Giphy API key not configured. Please add your API key to Web.config. Get a free key from <a href="https://developers.giphy.com/" target="_blank" style="color: #0066CC;">developers.giphy.com</a></div>';
                    return;
                }
                
                // Giphy API endpoint
                var apiUrl = 'https://api.giphy.com/v1/gifs/search?api_key=' + encodeURIComponent(giphyApiKey) + '&q=' + encodeURIComponent(searchTerm) + '&limit=24&rating=g';
                
                var xhr = new XMLHttpRequest();
                xhr.open('GET', apiUrl, true);
                xhr.onreadystatechange = function() {
                    if (xhr.readyState === 4) {
                        if (xhr.status === 200) {
                            try {
                                var response = JSON.parse(xhr.responseText);
                                if (response.data && response.data.length > 0) {
                                    displayGIFs(response.data);
                                } else {
                                    gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #999;">No GIFs found. Try a different search term.</div>';
                                }
                            } catch (e) {
                                console.error('Error parsing GIF response:', e);
                                gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #d32f2f;">Error loading GIFs. Please try again.</div>';
                            }
                        } else if (xhr.status === 403) {
                            gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #d32f2f;">Invalid or expired Giphy API key. Please check your API key in Web.config. Get a free key from <a href="https://developers.giphy.com/" target="_blank" style="color: #0066CC;">developers.giphy.com</a></div>';
                        } else {
                            gridContainer.innerHTML = '<div style="text-align: center; padding: 40px; color: #d32f2f;">Error searching for GIFs (Status: ' + xhr.status + '). Please try again.</div>';
                        }
                    }
                };
                xhr.send();
            }
            
            function displayGIFs(gifs) {
                gridContainer.innerHTML = '';
                
                var grid = document.createElement('div');
                grid.style.cssText = 'display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 12px;';
                
                gifs.forEach(function(gif) {
                    var gifItem = document.createElement('div');
                    gifItem.style.cssText = 'cursor: pointer; border: 2px solid transparent; border-radius: 8px; overflow: hidden; transition: all 0.2s; background: #f5f5f5;';
                    gifItem.onmouseover = function() { 
                        this.style.borderColor = '#0066CC';
                        this.style.transform = 'scale(1.05)';
                    };
                    gifItem.onmouseout = function() { 
                        this.style.borderColor = 'transparent';
                        this.style.transform = 'scale(1)';
                    };
                    gifItem.onclick = function() {
                        sendGIFMessage(gif.images.original.url);
                        closeGIFPicker();
                    };
                    
                    var img = document.createElement('img');
                    img.src = gif.images.fixed_height_small.url;
                    img.style.cssText = 'width: 100%; height: auto; display: block;';
                    img.alt = gif.title || 'GIF';
                    gifItem.appendChild(img);
                    grid.appendChild(gifItem);
                });
                
                gridContainer.appendChild(grid);
            }
            
            window.searchGIFs = searchGIFs;
        }
        
        function closeGIFPicker() {
            var modal = document.getElementById('gifPickerModal');
            if (modal) {
                document.body.removeChild(modal);
            }
            if (window.currentGIFModal) {
                try {
                    document.body.removeChild(window.currentGIFModal);
                } catch(e) {}
                window.currentGIFModal = null;
            }
        }


        function sendGIFMessage(gifUrl) {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'Family.aspx/SendChatGIF', true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response.d && response.d.success) {
                            loadMessages();
                            showMessageStatus('sent');
                        } else {
                            showMessageStatus('failed');
                        }
                    } catch (e) {
                        console.error('Error sending GIF:', e);
                        showMessageStatus('failed');
                    }
                }
            };
            xhr.send(JSON.stringify({ 
                familyId: chatState.familyId, 
                userId: chatState.userId, 
                gifUrl: gifUrl 
            }));
        }

        // Cleanup on page unload
        window.addEventListener('beforeunload', function() {
            if (chatState.pollingInterval) {
                clearInterval(chatState.pollingInterval);
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="Family.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <asp:Panel ID="pnlReviewLink" runat="server" Visible="false">
                            <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
                        </asp:Panel>
                        <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
                        <a href="RewardOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
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
        <div class="container" id="mainContainer" runat="server">
            <p class="page-subtitle" id="pSubtitle" runat="server">Create or join a family to get started</p>

            <!-- Family Info with Sidebar (if already in family) -->
            <asp:Panel ID="pnlFamilyInfo" runat="server" Visible="false">
                <div class="family-layout" id="familyLayout" runat="server">
                    <!-- Left Sidebar - Invite Code -->
                    <div class="family-sidebar-left">
                        <div class="sidebar-section">
                            <div class="sidebar-header">
                                <span class="sidebar-title">FAMILY CODE</span>
                            </div>
                            <div class="code-section-content">
                                <div class="code-label">Share with children</div>
                                <div class="code-display">
                                    <div class="family-code" id="familyCodeDisplay">
                                        <asp:Literal ID="litFamilyCode" runat="server"></asp:Literal>
                                    </div>
                                    <asp:Button ID="btnCopyCode" runat="server" 
                                        Text="Copy" 
                                        CssClass="btn-copy-code"
                                        OnClick="btnCopyCode_Click"
                                        OnClientClick="return false;" />
                                    <asp:Button ID="btnChangeCode" runat="server" 
                                        Text="Change" 
                                        CssClass="btn-change-code"
                                        OnClick="btnChangeCode_Click"
                                        UseSubmitBehavior="true"
                                        style="display: none;" />
                                    <asp:Button ID="btnChangeCodeHidden" runat="server" 
                                        Text="Change" 
                                        OnClick="btnChangeCode_Click"
                                        style="display: none;" />
                                </div>
                                <div class="code-description">Children can use this code to join your family</div>
                            </div>
                        </div>
                    </div>

                    <!-- Right Sidebar - Members -->
                    <div class="family-sidebar">
                        <!-- Parents Section -->
                        <div class="sidebar-section">
                            <div class="sidebar-header">
                                <span class="sidebar-title">PARENTS</span>
                                <span class="sidebar-count" id="parentCount" runat="server">0</span>
                            </div>
                            <div class="member-list" id="parentList">
                                <asp:Repeater ID="rptParents" runat="server" OnItemDataBound="rptParents_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="member-item parent-item" 
                                             data-user-id='<%# Eval("Id") %>'
                                             data-user-name='<%# Eval("FirstName") %> <%# Eval("LastName") %>'
                                             data-join-date='<%# GetJoinDateString(Eval("JoinDate")) %>'
                                             data-is-owner='<%# Convert.ToBoolean(Eval("IsOwner")) %>'
                                             oncontextmenu="showContextMenu(event, 'parent', <%# Eval("Id") %>, '<%# Eval("FirstName") %> <%# Eval("LastName") %>', <%# Convert.ToBoolean(Eval("IsOwner")) %>); return false;">
                                            <div class="member-avatar">
                                                <asp:PlaceHolder ID="phParentAvatar" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div class="member-info">
                                                <div class="member-name">
                                                    <%# Eval("FirstName") %> <%# Eval("LastName") %>
                                                    <asp:Label ID="lblOwnerBadge" runat="server" CssClass="owner-badge" Visible="false">OWNER</asp:Label>
                                                </div>
                                            </div>
                                            <div class="member-actions" runat="server" id="parentActions">
                                                <asp:Button ID="btnTransferOwnership" runat="server" 
                                                    Text="Transfer" 
                                                    CssClass="btn-transfer"
                                                    CommandName="Transfer"
                                                    CommandArgument='<%# Eval("Id") %>'
                                                    Visible="false"
                                                    OnClick="btnTransferOwnership_Click" />
                                                <asp:Button ID="btnKickParent" runat="server" 
                                                    Text="Kick" 
                                                    CssClass="btn-kick"
                                                    CommandName="Kick"
                                                    CommandArgument='<%# Eval("Id") %>'
                                                    Visible="false"
                                                    OnClick="btnKickParent_Click" />
                                            </div>
                                            <!-- Hover Tooltip -->
                                            <div class="parent-tooltip">
                                                <div class="tooltip-content">
                                                    <div class="tooltip-stat">
                                                        <span class="tooltip-label">Joined:</span>
                                                        <span class="tooltip-value"><asp:Literal ID="litJoinDate" runat="server"></asp:Literal></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <!-- Children Section -->
                        <div class="sidebar-section">
                            <div class="sidebar-header">
                                <span class="sidebar-title">CHILDREN</span>
                                <span class="sidebar-count" id="childCount" runat="server">0</span>
                            </div>
                            <div class="member-list" id="childList">
                                <asp:Repeater ID="rptChildren" runat="server" OnItemDataBound="rptChildren_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="member-item child-item" 
                                             data-user-id='<%# Eval("Id") %>'
                                             data-user-name='<%# Eval("FirstName") %> <%# Eval("LastName") %>'
                                             data-points='<%# Eval("TotalPoints") %>'
                                             data-completed='<%# Eval("CompletedTasks") %>'
                                             data-failed='<%# Eval("FailedTasks") %>'
                                             oncontextmenu="showContextMenu(event, 'child', <%# Eval("Id") %>, '<%# Eval("FirstName") %> <%# Eval("LastName") %>'); return false;">
                                            <div class="member-avatar">
                                                <asp:PlaceHolder ID="phChildAvatar" runat="server"></asp:PlaceHolder>
                                            </div>
                                            <div class="member-info">
                                                <div class="member-name">
                                                    <%# Eval("FirstName") %> <%# Eval("LastName") %>
                                                </div>
                                            </div>
                                            <!-- Hover Tooltip -->
                                            <div class="child-tooltip">
                                                <div class="tooltip-content">
                                                    <div class="tooltip-stat">
                                                        <span class="tooltip-label">Points:</span>
                                                        <span class="tooltip-value"><%# Convert.ToInt32(Eval("TotalPoints")).ToString("N0") %></span>
                                                    </div>
                                                    <div class="tooltip-stat">
                                                        <span class="tooltip-label">Completed:</span>
                                                        <span class="tooltip-value completed"><%# Eval("CompletedTasks") %></span>
                                                    </div>
                                                    <div class="tooltip-stat">
                                                        <span class="tooltip-label">Failed:</span>
                                                        <span class="tooltip-value failed"><%# Eval("FailedTasks") %></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>

                    <!-- Main Content Area - Chat -->
                    <div class="family-main">
                        <!-- Chat Container -->
                        <div class="chat-container" id="chatContainer">
                            <!-- Chat Header with Family Name -->
                            <div class="chat-header">
                                <h1 class="chat-family-name">
                                    <asp:Literal ID="litFamilyNameHeader" runat="server" Text="Family Management"></asp:Literal>
                                </h1>
                            </div>
                            <!-- Chat Messages Area -->
                            <div class="chat-messages" id="chatMessages">
                                <div class="chat-loading" id="chatLoading">Loading messages...</div>
                                <div class="chat-empty" id="chatEmpty" style="display: none;">
                                    <div class="empty-state">
                                        <div class="empty-icon">💬</div>
                                        <div class="empty-text">No messages yet. Start the conversation!</div>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Chat Input Area -->
                            <div class="chat-input-container">
                                <div class="chat-input-wrapper">
                                    <button type="button" class="chat-btn chat-btn-upload" id="btnUploadImage" title="Upload Photo">
                                        <span>&#128247;</span>
                                    </button>
                                    <input type="file" id="fileUploadImage" accept="image/*" style="display: none;" />
                                    <button type="button" class="chat-btn chat-btn-gif" id="btnGIF" title="Send GIF">
                                        <span>GIF</span>
                                    </button>
                                    <div class="chat-input-box">
                                        <textarea id="txtMessage" class="chat-input" placeholder="Type a message..." rows="1" maxlength="5000"></textarea>
                                        <div class="chat-reply-preview" id="replyPreview" style="display: none;">
                                            <div class="reply-preview-content">
                                                <div class="reply-preview-header">
                                                    <span class="reply-preview-name" id="replyPreviewName"></span>
                                                    <button type="button" class="reply-preview-close" id="btnCancelReply">×</button>
                                                </div>
                                                <div class="reply-preview-text" id="replyPreviewText"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" class="chat-btn chat-btn-send" id="btnSendMessage" title="Send">
                                        <span>&#10148;</span>
                                    </button>
                                </div>
                                <div class="chat-upload-progress" id="uploadProgress" style="display: none;">
                                    <div class="progress-bar">
                                        <div class="progress-fill" id="progressFill"></div>
                                    </div>
                                    <span class="progress-text" id="progressText">Uploading...</span>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Messages -->
                        <asp:Label ID="lblFamilyError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                        <asp:Label ID="lblFamilySuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                    </div>
                </div>
            </asp:Panel>

            <!-- Custom Confirmation Modal -->
            <div id="confirmModal" class="modal-overlay" onclick="if(event.target === this) closeConfirmModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <div id="modalIcon" class="modal-icon warning">&#9888;</div>
                        <h3 id="modalTitle" class="modal-title">Confirm Action</h3>
                    </div>
                    <p id="modalMessage" class="modal-message">Are you sure you want to proceed?</p>
                    <div class="modal-actions">
                        <button type="button" class="modal-btn modal-btn-cancel" onclick="closeConfirmModal();">Cancel</button>
                        <button type="button" id="modalConfirmBtn" class="modal-btn modal-btn-confirm" onclick="confirmModalAction();">Confirm</button>
                    </div>
                </div>
            </div>

            <!-- Right-Click Context Menu -->
            <div id="contextMenu" class="context-menu">
                <div class="context-menu-item" id="contextMenuKick" onclick="contextMenuKick()">
                    <span class="context-menu-icon">&#10006;</span>
                    <span class="context-menu-text">Kick Out</span>
                </div>
            </div>

            <!-- Toast Message -->
            <div id="messageToast" class="message-toast">
                <span class="message-toast-text"></span>
                <button type="button" class="message-toast-close" onclick="closeToast()">&times;</button>
            </div>

            <!-- Hidden fields for chat -->
            <asp:HiddenField ID="hidFamilyId" runat="server" Value="" />
            <asp:HiddenField ID="hidUserId" runat="server" Value="" />
            <asp:HiddenField ID="hidGiphyApiKey" runat="server" Value="" />

            <!-- Hidden buttons for context menu actions -->
            <asp:Button ID="btnKickChildHidden" runat="server" 
                OnClick="btnKickChild_Click" 
                style="display: none;" />
            <asp:HiddenField ID="hidKickUserId" runat="server" Value="" />
            <asp:HiddenField ID="hidKickUserType" runat="server" Value="" />

            <!-- Create/Join Tabs (if not in family) -->
            <asp:Panel ID="pnlFamilyActions" runat="server" Visible="false">
                <div class="tabs">
                    <button type="button" class="tab active" id="tabCreate" onclick="showTab('create')">Create Family</button>
                    <button type="button" class="tab" id="tabJoin" onclick="showTab('join')">Join Family</button>
                </div>

                <!-- Create Family Form -->
                <div id="createForm" class="form-container">
                    <h2 style="margin-bottom: 20px; color: #333;">Create a New Family</h2>
                    
                    <!-- Information Panel -->
                    <div style="background-color: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px; margin-bottom: 20px; border-radius: 5px;">
                        <div style="display: flex; align-items: flex-start; gap: 10px;">
                            <div style="font-size: 20px; color: #2196f3; flex-shrink: 0;">&#8505;</div>
                            <div style="flex: 1;">
                                <div style="font-weight: 600; color: #1976d2; margin-bottom: 8px; font-size: 15px;">About the Family Treasury System</div>
                                <div style="color: #333; font-size: 14px; line-height: 1.6;">
                                    <p style="margin: 0 0 8px 0;">When you create a family, you'll receive <strong>1,000,000 points</strong> in your family treasury.</p>
                                    <p style="margin: 0 0 8px 0;">&bull; <strong>Task Rewards:</strong> When you create tasks and set point rewards, those points are deducted from the treasury.</p>
                                    <p style="margin: 0 0 8px 0;">&bull; <strong>Reward Purchases:</strong> When children purchase rewards from the reward store, the points they spend are returned to the treasury.</p>
                                    <p style="margin: 0;">&bull; <strong>Balance Management:</strong> Keep track of your treasury balance to ensure you have enough points for task rewards.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtCreateFamilyName">Family Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtCreateFamilyName" runat="server" CssClass="form-control" placeholder="Enter family name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCreateFamilyName" runat="server" ControlToValidate="txtCreateFamilyName" 
                            ErrorMessage="Family name is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtCreatePinCode">6-Digit PIN Code <span class="required">*</span></label>
                        <asp:TextBox ID="txtCreatePinCode" runat="server" CssClass="form-control" MaxLength="6" placeholder="000000" TextMode="SingleLine"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCreatePinCode" runat="server" ControlToValidate="txtCreatePinCode" 
                            ErrorMessage="PIN code is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revCreatePinCode" runat="server" ControlToValidate="txtCreatePinCode" 
                            ValidationExpression="^\d{6}$" ErrorMessage="PIN must be 6 digits" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RegularExpressionValidator>
                    </div>
                    <asp:Button ID="btnCreateFamily" runat="server" Text="Create Family" CssClass="btn-submit" ValidationGroup="CreateFamily" OnClick="btnCreateFamily_Click" OnClientClick="console.log('Create Family button clicked'); showLoadingCreate(); return true;" UseSubmitBehavior="true" />
                    <asp:Label ID="lblCreateError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                    <asp:Label ID="lblCreateSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                </div>

                <!-- Join Family Form -->
                <div id="joinForm" class="form-container hidden">
                    <h2 style="margin-bottom: 20px; color: #333;">Join an Existing Family</h2>
                    <div class="form-group">
                        <label for="txtJoinFamilyName">Family Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtJoinFamilyName" runat="server" CssClass="form-control" placeholder="Enter family name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvJoinFamilyName" runat="server" ControlToValidate="txtJoinFamilyName" 
                            ErrorMessage="Family name is required" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtJoinPinCode">6-Digit PIN Code <span class="required">*</span></label>
                        <asp:TextBox ID="txtJoinPinCode" runat="server" CssClass="form-control" MaxLength="6" placeholder="000000" TextMode="SingleLine"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvJoinPinCode" runat="server" ControlToValidate="txtJoinPinCode" 
                            ErrorMessage="PIN code is required" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revJoinPinCode" runat="server" ControlToValidate="txtJoinPinCode" 
                            ValidationExpression="^\d{6}$" ErrorMessage="PIN must be 6 digits" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RegularExpressionValidator>
                    </div>
                    <asp:Button ID="btnJoinFamily" runat="server" Text="Join Family" CssClass="btn-submit" ValidationGroup="JoinFamily" OnClick="btnJoinFamily_Click" OnClientClick="console.log('Join Family button clicked'); showLoadingJoin(); return true;" UseSubmitBehavior="true" />
                    <asp:Label ID="lblJoinError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                    <asp:Label ID="lblJoinSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

