"""
Counter API Implementation
"""
from flask import Flask, jsonify
from . import status

app = Flask(__name__)

COUNTERS = {}

def counter_exists(name):
  """Check if counter exists"""
  return name in COUNTERS

@app.route('/counters/<name>', methods=["GET","POST"])
def create_counter(name):
    """Create a counter"""
    if counter_exists(name):
        return jsonify({"error": f"Counter {name} already exists"}), status.HTTP_409_CONFLICT
    COUNTERS[name] = 0
    return jsonify({name: COUNTERS[name]}), status.HTTP_201_CREATED


    ## ===========================
    # Test: Deleting counters that exist and dont exist
    # Author: Adrian Janda
    # Date: 2025-09-11
    # Description: creating deleteing function
    # ===========================
@app.route('/counters/<name>', methods=['DELETE'])
def delete_counter(name):
    """Delete a counter"""
    if not counter_exists(name):
        return jsonify({"error": f"Counter {name} does not exists"}), status.HTTP_404_NOT_FOUND
    
    del COUNTERS[name]
    return '', status.HTTP_204_NO_CONTENT