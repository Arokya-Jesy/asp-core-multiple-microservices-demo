import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/gateway';

function Orders() {
  const [orders, setOrders] = useState([]);
  const [products, setProducts] = useState([]);
  const [selectedProduct, setSelectedProduct] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchOrders();
    fetchProducts();
  }, []);

  const fetchOrders = async () => {
    try {
      const { data } = await api.get('/orders');
      setOrders(data);
    } catch (err) {
      setError('Failed to fetch orders');
    }
  };

  const fetchProducts = async () => {
    try {
      const { data } = await api.get('/products');
      setProducts(data);
    } catch (err) {
      setError('Failed to fetch products');
    }
  };

  const handleCreateOrder = async (e) => {
    e.preventDefault();
    try {
      await api.post('/orders', {
        productId: parseInt(selectedProduct),
        quantity: quantity
      });
      setSelectedProduct('');
      setQuantity(1);
      fetchOrders();
    } catch (err) {
      setError('Failed to create order');
    }
  };

  return (
    <div style={{ maxWidth: '800px', margin: '50px auto', padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
        <h2>My Orders</h2>
        <div>
          <button onClick={() => navigate('/products')} style={{ marginRight: '10px' }}>Products</button>
          <button onClick={() => { localStorage.clear(); navigate('/'); }}>Logout</button>
        </div>
      </div>
      
      {error && <p style={{ color: 'red' }}>{error}</p>}

      <form onSubmit={handleCreateOrder} style={{ marginBottom: '30px', padding: '15px', border: '1px solid #ddd' }}>
        <h3>Create New Order</h3>
        <div style={{ marginBottom: '10px' }}>
          <select 
            value={selectedProduct} 
            onChange={(e) => setSelectedProduct(e.target.value)}
            required
            style={{ width: '100%', padding: '8px' }}
          >
            <option value="">Select Product</option>
            {products.map(p => (
              <option key={p.id} value={p.id}>{p.name} - ${p.price}</option>
            ))}
          </select>
        </div>
        <div style={{ marginBottom: '10px' }}>
          <input
            type="number"
            min="1"
            value={quantity}
            onChange={(e) => setQuantity(parseInt(e.target.value))}
            placeholder="Quantity"
            style={{ width: '100%', padding: '8px' }}
          />
        </div>
        <button type="submit" style={{ padding: '10px 20px' }}>Place Order</button>
      </form>

      <h3>Order History</h3>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Order ID</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Product</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Price</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Quantity</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Total</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Date</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{order.id}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{order.productName}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>${order.productPrice}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{order.quantity}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>${order.totalPrice}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                {new Date(order.orderDate).toLocaleDateString()}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Orders;
