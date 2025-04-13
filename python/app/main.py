from fastapi import FastAPI
from app.test_runner import run_all_tests

app = FastAPI()

@app.get("/run-tests")
async def run_tests_endpoint():
    result = run_all_tests()
    return {"status": "completed", "result": result}
