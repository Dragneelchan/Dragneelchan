<?php
  session_start();
  require("connect.php");
  $sql = "SELECT * FROM menu;";
  $objQuery = mysqli_query($conn, $sql);
?>

<!DOCTYPE html>
<html>
  <head>
    <title>Menu</title>
  </head>
  <body>
    <h2>Menu</h2>
    <ul id="menu">
      <?php while ($objResult = mysqli_fetch_assoc($objQuery)) { ?>
      <li>
        <span><?php echo $objResult["menu_name"]; ?></span>
        <button class="add-to-cart" data-id="<?php echo $objResult["menu_id"]; ?>" data-name="<?php echo $objResult["menu_name"]; ?>" data-price="<?php echo $objResult["menu_price"]; ?>">Add to cart</button>
      </li>
      <?php } ?>
    </ul>

    <h2>Cart</h2>
    <ul id="cart"></ul>
    <button id="checkout">Checkout</button>

    <script>
      // Define variables
      const menuItems = document.querySelectorAll('#menu li');
      const cart = document.querySelector('#cart');
      const checkoutBtn = document.querySelector('#checkout');

      // Initialize cart
      let cartItems = [];

      // Add event listeners to menu items
      menuItems.forEach(item => {
        item.querySelector('.add-to-cart').addEventListener('click', addToCart);
      });

      // Add item to cart
      function addToCart(e) {
        const itemId = e.target.getAttribute('data-id');
        const itemName = e.target.getAttribute('data-name');
        const itemPrice = e.target.getAttribute('data-price');

        // Check if item is already in cart
        const existingItem = cartItems.find(item => item.id === itemId);
        if (existingItem) {
          existingItem.quantity++;
        } else {
          cartItems.push({
            id: itemId,
            name: itemName,
            price: itemPrice,
            quantity: 1
          });
        }

        // Update cart display
        renderCart();
      }

      // Render cart
      function renderCart() {
        cart.innerHTML = '';

        // Loop through cart items and add to cart display
        cartItems.forEach(item => {
          const li = document.createElement('li');
          li.innerHTML = `${item.name} - $${item.price} x ${item.quantity}`;
          cart.appendChild(li);
        });
      }

      // Calculate total price of items in cart
      function calculateTotal() {
        let total = 0;
        cartItems.forEach(item => {
          total += item.price * item.quantity;
        });
        return total;
      }

      // Add event listener to checkout button
      checkoutBtn.addEventListener('click', () => {
        const total = calculateTotal();
        alert(`Total: $${total}`);
      });
    </script>
  </body>
</html>