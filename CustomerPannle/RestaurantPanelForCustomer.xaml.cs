﻿using Restaurant_Manager.Models;
using System.Windows;
using System.Linq;
using Restaurant_Manager.DAL;
using System;
using System.Data.Entity;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MailKit.Security;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using static System.Net.WebRequestMethods;

namespace Restaurant_Manager.CustomerPannle
{
    public partial class RestaurantPanelForCustomer : Window
    {
        private Restaurant _restaurant;
        private User _currentUser;
        private ObservableCollection<Stuff> cartItems = new ObservableCollection<Stuff>();

        public RestaurantPanelForCustomer(Restaurant restaurant, User currentUser)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _currentUser = currentUser;
            LoadMenu();
        }

        private void LoadMenu()
        {
            using (var context = new RestaurantContext())
            {
                var menuItems = context.Stuffs
                    .Where(s => s.Resturant.Id == _restaurant.Id)
                    .ToList();

                var groupedMenuItems = menuItems.GroupBy(m => m.fType)
                    .Select(g => new { Type = g.Key, Items = g.ToList() })
                    .ToList();

                MenuListView.ItemsSource = groupedMenuItems;
            }
        }

        private void MenuListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MenuListView.SelectedItem is Stuff selectedStuff)
            {
                LoadComments(selectedStuff);
            }
        }

        private void LoadComments(Stuff stuff)
        {
            using (var context = new RestaurantContext())
            {
                var comments = context.Comments
                    .Where(c => c.Stuff.Id == stuff.Id)
                    .Include(c => c.Users)
                    .ToList();

                CommentTreeView.ItemsSource = comments;
            }
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (MenuListView.SelectedItem is Stuff selectedStuff && !string.IsNullOrEmpty(NewCommentTextBox.Text))
            {
                using (var context = new RestaurantContext())
                {
                    var newComment = new Comment
                    {
                        Details = NewCommentTextBox.Text,
                        Date = DateTime.Now,
                        IsEdited = false,
                        Users = _currentUser,
                        Stuff = selectedStuff
                    };

                    context.Comments.Add(newComment);
                    context.SaveChanges();

                    LoadComments(selectedStuff); // Refresh comments
                }
            }
        }

        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment && selectedComment.Users.Id == _currentUser.Id)
            {
                var newDetails = "New comment details"; // Get from user input
                EditComment(selectedComment.Id, newDetails);
            }
        }

        private void DeleteComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment && selectedComment.Users.Id == _currentUser.Id)
            {
                DeleteComment(selectedComment.Id);
            }
        }

        private void EditComment(int commentId, string newDetails)
        {
            using (var context = new RestaurantContext())
            {
                var comment = context.Comments.Find(commentId);
                if (comment != null)
                {
                    comment.Details = newDetails;
                    comment.IsEdited = true;
                    context.SaveChanges();
                }
            }
        }

        private void DeleteComment(int commentId)
        {
            using (var context = new RestaurantContext())
            {
                var comment = context.Comments.Find(commentId);
                if (comment != null)
                {
                    context.Comments.Remove(comment);
                    context.SaveChanges();
                }
            }
        }

        private void CommentTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment)
            {
                var dialogResult = MessageBox.Show($"Selected Comment: {selectedComment.Details}\n\n" +
                                                   $"Date: {selectedComment.Date}\n" +
                                                   $"Edited: {(selectedComment.IsEdited ? "Yes" : "No")}",
                                                   "Comment Details", MessageBoxButton.OK, MessageBoxImage.Information);

                if (dialogResult == MessageBoxResult.OK && selectedComment.Users.Id == _currentUser.Id)
                {
                    var editResult = MessageBox.Show("Do you want to edit this comment?", "Edit Comment", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (editResult == MessageBoxResult.Yes)
                    {
                        var newDetails = "New comment details"; // Replace with actual edit logic
                        EditComment(selectedComment.Id, newDetails);
                    }
                }
            }
        }

        private void ReplyComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment)
            {
                var replyDetails = "Reply comment details"; // Get from user input
                AddReply(selectedComment.Id, replyDetails);
            }
        }

        private void AddReply(int commentId, string replyDetails)
        {
            using (var context = new RestaurantContext())
            {
                var parentComment = context.Comments.Find(commentId);
                if (parentComment != null)
                {
                    parentComment.Answer = replyDetails;
                    context.SaveChanges();

                    // Refresh comments to show the answer
                    LoadComments(parentComment.Stuff);
                }
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (MenuListView.SelectedItem is Stuff selectedStuff)
            {
                var existingCartItem = cartItems.FirstOrDefault(item => item.Id == selectedStuff.Id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++;
                }
                else
                {
                    selectedStuff.Quantity = 1;
                    cartItems.Add(selectedStuff);
                }

                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);
            TotalPriceTextBlock.Text = totalPrice.ToString();
        }

        private void ProceedToPayment_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int otp = random.Next(10000, 99999);
            string userEmail = _currentUser.Email;

            // Call SendMessage to send OTP via email
            SendMessage(userEmail, otp);

            // Display a message to the user
            MessageBox.Show($"OTP sent to {userEmail}. Please check your email.", "OTP Sent", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optionally, display the OTP for testing purposes
            MessageBox.Show($"OTP: {otp}", "OTP for Testing", MessageBoxButton.OK, MessageBoxImage.Information);

            // Show the OTP grid or perform other actions as needed
            otpGrid.Visibility = Visibility.Visible;
        }

        public static void SendMessage(string email, int otp)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("salirezamir", "salirezamir@yandex.com"));
            mimeMessage.To.Add(new MailboxAddress("DEV TEST OTP", email));
            mimeMessage.Subject = "DEV TEST OTP";
            mimeMessage.Body = new TextPart("plain")
            {
                Text = $"***DEV TEST OTP***\nOTP is : {otp}"
            };

            using (var client = new SmtpClient(new ProtocolLogger("smtp.log")))
            {
                client.Connect("smtp.yandex.com", 465, SecureSocketOptions.SslOnConnect);

                client.Authenticate("salirezamir", "gtvmchybehgvlusz");

                client.Send(mimeMessage);

                client.Disconnect(true);
            }
        }

    }
}
