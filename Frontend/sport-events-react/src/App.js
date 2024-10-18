
import './App.css';

import Matches from "./pages/Matches"
import React from 'react';
import { BrowserRouter as Router, Route, Switch, Routes } from 'react-router-dom';
import SingleMatch from './pages/SingleMatch';

function App() {
  return (
    <Router>
      <div>
        <Routes>
          <Route path="/matches" element={<Matches />} />
          <Route path="/matches/:id" element={<SingleMatch />} />
        </Routes>

        {/* Add more routes as needed */}

      </div>
    </Router>
  );
}

export default App;
