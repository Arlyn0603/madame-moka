let cart = [];
let total = 0;

// 🛒 Agregar un producto al carrito
function openCartPopup(name, description, price, image) {
    // Crear el popup
    const popup = document.createElement("div");
    popup.classList.add("cart-modal");
    popup.innerHTML = `
        <div class="cart-modal-content">
            <span class="close-btn" onclick="closeCartPopup()">&times;</span>
            <h3>${name}</h3>
            <p>${description}</p>
            <p class="product-price">${price}</p>
            <div class="quantity-controls">
                <button onclick="updateQuantity(-1)">➖</button>
                <input type="number" id="cart-quantity" value="1" min="1">
                <button onclick="updateQuantity(1)">➕</button>
            </div>
            <button class="add-to-cart-btn" onclick="addToCart('${name}', '${description}', '${price}', '${image}')">
                <i class="fa-solid fa-shopping-cart"></i>
            </button>
        </div>
    `;

    // Agregar el popup al body
    document.body.appendChild(popup);
}

// 🛑 Cerrar el popup
function closeCartPopup() {
    document.querySelector(".cart-modal").remove();
}

// 🔢 Actualizar la cantidad con botones ➖ ➕
function updateQuantity(value) {
    let quantityInput = document.getElementById("cart-quantity");
    let currentValue = parseInt(quantityInput.value);
    if (!isNaN(currentValue)) {
        let newValue = currentValue + value;
        if (newValue >= 1) {
            quantityInput.value = newValue;
        }
    }
}


// 🔄 Actualizar el carrito
function updateCart() {
    const cartItemsContainer = document.getElementById("cart-items");
    const cartTotalElement = document.getElementById("cart-total");

    if (!cartItemsContainer || !cartTotalElement) {
        console.error("No se encontró el contenedor del carrito o el total.");
        return;
    }

    cartItemsContainer.innerHTML = "";
    total = 0;

    cart.forEach((product, index) => {
        total += parseFloat(product.total);
        cartItemsContainer.innerHTML += `
            <div class="cart-item">
                <img src="${product.image}" alt="${product.name}" class="cart-img">
                <div class="cart-details">
                    <h4>${product.name}</h4>
                    <p>${product.description}</p>
                    <p>Cantidad: ${product.quantity}</p>
                    <p>Subtotal: ₡${product.total}</p>
                    <button onclick="removeFromCart(${index})">❌ Eliminar</button>
                </div>
            </div>
        `;
    });

    cartTotalElement.innerText = `₡${total.toFixed(2)}`;
}

// ❌ Eliminar un producto del carrito
function removeFromCart(index) {
    cart.splice(index, 1);
    updateCart();
}

// 🛒 Simular el pago
function checkout() {
    if (cart.length === 0) {
        alert("El carrito está vacío.");
        return;
    }

    alert("Pedido procesado con éxito. Total: ₡" + total.toFixed(2));
    cart = [];
    updateCart();
}
function addToCart(name, description, price, image) {
    let quantity = parseInt(document.getElementById("cart-quantity").value);
    const productTotal = parseFloat(price.replace("₡", "").replace(",", "")) * quantity;

    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    cart.push({
        name,
        description,
        price,
        quantity,
        total: productTotal.toFixed(2),
        image
    });

    localStorage.setItem("cart", JSON.stringify(cart));

    // ✅ Redirigir al carrito
    window.location.href = "/Carrito/Index";
}
