import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/gateway';

function Products() {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({ name: '', price: '' });
  const navigate = useNavigate();
  const role = localStorage.getItem('role') || 'user';

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const { data } = await api.get('/products');
      setProducts(data);
    } catch (err) {
      setError('Failed to fetch products');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        await api.put(`/products/${editingId}`, formData);
      } else {
        await api.post('/products', formData);
      }
      setFormData({ name: '', price: '' });
      setEditingId(null);
      setShowForm(false);
      fetchProducts();
    } catch (err) {
      setError('Failed to save product');
    }
  };

  const handleEdit = (product) => {
    setFormData({ name: product.name, price: product.price });
    setEditingId(product.id);
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Delete this product?')) {
      try {
        await api.delete(`/products/${id}`);
        fetchProducts();
      } catch (err) {
        setError('Failed to delete product');
      }
    }
  };

  const handleLogout = () => {
    localStorage.clear();
    navigate('/');
  };

  return (
    <div style={{ maxWidth: '800px', margin: '50px auto', padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
        <h2>Products</h2>
        <div>
          <button onClick={() => navigate('/orders')} style={{ marginRight: '10px' }}>My Orders</button>
          <span style={{ marginRight: '10px' }}>Role: {role}</span>
          <button onClick={handleLogout}>Logout</button>
        </div>
      </div>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      
      {role === 'admin' && (
        <button onClick={() => { setShowForm(!showForm); setEditingId(null); setFormData({ name: '', price: '' }); }} style={{ marginBottom: '20px' }}>
          {showForm ? 'Cancel' : 'Add Product'}
        </button>
      )}

      {showForm && role === 'admin' && (
        <form onSubmit={handleSubmit} style={{ marginBottom: '20px', padding: '10px', border: '1px solid #ddd' }}>
          <div style={{ marginBottom: '10px' }}>
            <input
              type="text"
              placeholder="Product Name"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
              style={{ width: '100%', padding: '8px' }}
            />
          </div>
          <div style={{ marginBottom: '10px' }}>
            <input
              type="number"
              step="0.01"
              placeholder="Price"
              value={formData.price}
              onChange={(e) => setFormData({ ...formData, price: e.target.value })}
              required
              style={{ width: '100%', padding: '8px' }}
            />
          </div>
          <button type="submit" style={{ padding: '8px 16px' }}>
            {editingId ? 'Update' : 'Create'}
          </button>
        </form>
      )}

      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>ID</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Name</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Price</th>
            {role === 'admin' && <th style={{ border: '1px solid #ddd', padding: '8px' }}>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{product.id}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{product.name}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>${product.price}</td>
              {role === 'admin' && (
                <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                  <button onClick={() => handleEdit(product)} style={{ marginRight: '5px' }}>Edit</button>
                  <button onClick={() => handleDelete(product.id)}>Delete</button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Products;
