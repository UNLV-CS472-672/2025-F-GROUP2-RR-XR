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

    
    # ===========================
    # Test: check_duplicated_counter
    # Author: Alex Yamasaki
    # Date: 2025-09-10
    # Description: PREVENT DUPLCIATED counters
    # ===========================
    def check_duplicated_counter(self, client):
        #Assume creation is worked
        client.post('/counters/foo')
        result = client.post('/counters/foo')
        assert result.status_code == status.HTTP_409_CONFLICT
        assert b"Already exists" in result.data
        
    # ===========================
    # Test: List All Counters
    # Author: Gerhod Moreno
    # Date: 2025-09-12
    # Description: Be able to list out all counters
    # ===========================
    def test_list_all_counts(self, client):
        result = client.get('/counters')
        #check to see if response was ok
        assert result.status_code == status.HTTP_200_OK
        counters = result.get_json()

        #check that there is atleast one counter 
        assert len(counters) > 0

    ## ===========================
    # Test: Deleting counters that exist and dont exist
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

    ## ===========================
    # Test: Increment a counter
    # Author: Christopher Vuong
    # Date: 2025-09-12
    # Description: Ensure counters can be incremented
    # ===========================
    def test_increment_counters(self, client):
        """It should increment the counter"""
        #increments foo and checks if the value goes up by 1
        init = client.post('/counters/foo')
        result = client.put('/counters/foo')
        assert result.status_code == status.HTTP_200_OK
        assert result.json['foo'] == init.json['foo'] + 1

        #tries to increment a non-existent counter
        fail = client.put('/counters/foo2')
        assert fail.status_code == status.HTTP_404_NOT_FOUND
        

