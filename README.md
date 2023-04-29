# WebStore

WebStore is a web application written in Asp.Net CORE MVC for . Net 5, which allows you to create categories and products, add them to the cart, confirm orders and send data about orders to the seller.

## Using

### User Roles

The application supports two user roles:

- Administrator - can add, delete, edit categories and products, view order information.
- Ordinary user - can view categories and products, add them to cart, confirm orders.

### Create Categories and Products

To create categories and products you need to login under the administrator account and press "Content Management" on the navigation panel. On this page you can add a new category or product, edit or delete existing ones.

### Add Products to Cart

To add a product to the cart, you need to select the right product, you need to click on "View Details", and then click the add button. After that the product will be added to the cart, which can be viewed by clicking on the "Cart" on the navigation panel.

### Order confirmation

After all the necessary products have been added to the cart, the user must specify the information about himself and confirm the order. After confirmation of the order, the details of the order will be sent to the seller.

### View Order Information

The administrator can view all orders in the "Inquiry Managment" menu. This page displays a list of all orders made by regular users.
