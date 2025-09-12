"""
Test Cases for Counter Web Service

Create a service that can keep a track of multiple counters
- API must be RESTful - see the status.py file. Following these guidelines, you can make assumptions about
how to call the web service and assert what it should return.
- The endpoint should be called /counters
- When creating a counter, you must specify the name in the path.
- Duplicate names must return a conflict error code.
- The service must be able to update a counter by name.
- The service must be able to read the counter
"""

import pytest
from src import app
from src import status

@pytest.fixture()
def client():
    """Fixture for Flask test client"""
    return app.test_client()

@pytest.mark.usefixtures("client")
class TestCounterEndpoints:
    """Test cases for Counter API"""

    def test_create_counter(self, client):
        """It should create a counter"""
        result = client.post('/counters/foo')
        assert result.status_code == status.HTTP_201_CREATED

    ## ===========================
    # Test: Account email updated
    # Author: Adrian Janda
    # Date: 2025-09-11
    # Description: testing deleting counters
    # ===========================
    def test_delete_counters(self,client):
        client.post('/counters/foo')
        result = client.delete('/counters/foo')
        assert result.status_code == status.HTTP_204_NO_CONTENT

        result = client.delete('/counters/foo')
        assert result.status_code == status.HTTP_404_NOT_FOUND
        
