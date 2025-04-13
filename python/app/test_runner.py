import pytest

def run_all_tests():
    result = pytest.main(["-s", "app/test_person_flow.py"])
    return result
