export default function ProductCard({ product, onAddToCart }: any) {
  const effectivePrice = product.discountPrice || product.price;
  const discount = product.discountPrice
    ? Math.round(((product.price - product.discountPrice) / product.price) * 100)
    : 0;

  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition">
      {/* Image */}
      <div className="relative bg-gray-200 h-48 flex items-center justify-center">
        {product.images?.[0] ? (
          <img src={product.images[0]} alt={product.name} className="w-full h-full object-cover" />
        ) : (
          <span className="text-gray-400">No Image</span>
        )}
        {discount > 0 && (
          <div className="absolute top-2 right-2 bg-red-500 text-white px-2 py-1 rounded text-sm font-bold">
            -{discount}%
          </div>
        )}
      </div>

      {/* Content */}
      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 truncate">{product.name}</h3>
        <p className="text-gray-600 text-sm mt-1 line-clamp-2">{product.description}</p>

        {/* Rating */}
        <div className="mt-2 flex items-center">
          <div className="flex text-yellow-400">
            {[...Array(5)].map((_, i) => (
              <span key={i}>{i < Math.round(product.averageRating) ? '★' : '☆'}</span>
            ))}
          </div>
          <span className="text-gray-600 text-sm ml-2">({product.averageRating.toFixed(1)})</span>
        </div>

        {/* Price */}
        <div className="mt-3 flex items-baseline space-x-2">
          <span className="text-2xl font-bold text-gray-900">${effectivePrice.toFixed(2)}</span>
          {product.discountPrice && (
            <span className="text-gray-500 line-through">${product.price.toFixed(2)}</span>
          )}
        </div>

        {/* Stock */}
        <p className={`text-sm mt-2 ${product.stockQuantity > 0 ? 'text-green-600' : 'text-red-600'}`}>
          {product.stockQuantity > 0 ? `${product.stockQuantity} in stock` : 'Out of stock'}
        </p>

        {/* Add to Cart Button */}
        <button
          onClick={() => onAddToCart(product)}
          disabled={product.stockQuantity === 0}
          className="mt-4 w-full py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition"
        >
          {product.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock'}
        </button>
      </div>
    </div>
  );
}
