export var UserRole;
(function (UserRole) {
    UserRole[UserRole["SuperAdmin"] = 1] = "SuperAdmin";
    UserRole[UserRole["Vendor"] = 2] = "Vendor";
    UserRole[UserRole["Customer"] = 3] = "Customer";
})(UserRole || (UserRole = {}));
export var ProductStatus;
(function (ProductStatus) {
    ProductStatus[ProductStatus["Draft"] = 1] = "Draft";
    ProductStatus[ProductStatus["Active"] = 2] = "Active";
    ProductStatus[ProductStatus["OutOfStock"] = 3] = "OutOfStock";
    ProductStatus[ProductStatus["Archived"] = 4] = "Archived";
})(ProductStatus || (ProductStatus = {}));
export var OrderStatus;
(function (OrderStatus) {
    OrderStatus[OrderStatus["Pending"] = 1] = "Pending";
    OrderStatus[OrderStatus["Confirmed"] = 2] = "Confirmed";
    OrderStatus[OrderStatus["Processing"] = 3] = "Processing";
    OrderStatus[OrderStatus["Shipped"] = 4] = "Shipped";
    OrderStatus[OrderStatus["Delivered"] = 5] = "Delivered";
    OrderStatus[OrderStatus["Cancelled"] = 6] = "Cancelled";
    OrderStatus[OrderStatus["Refunded"] = 7] = "Refunded";
})(OrderStatus || (OrderStatus = {}));
