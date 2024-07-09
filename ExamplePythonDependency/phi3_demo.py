﻿import torch 
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline 

def phi3_inference_demo(user_message: str, system_message: str = "You are a helpful AI assistant.", temperature: float = 0.0) -> str:
    torch.random.manual_seed(0) 
    model = AutoModelForCausalLM.from_pretrained( 
        "microsoft/Phi-3-mini-4k-instruct",  
        device_map="cuda",  
        torch_dtype="auto",  
        trust_remote_code=True,  
    ) 

    tokenizer = AutoTokenizer.from_pretrained("microsoft/Phi-3-mini-4k-instruct") 

    messages = [ 
        {"role": "system", "content": system_message}, 
        {"role": "user", "content": user_message}, 
    ] 

    pipe = pipeline( 
        "text-generation", 
        model=model, 
        tokenizer=tokenizer, 
    ) 

    generation_args = { 
        "max_new_tokens": 500, 
        "return_full_text": False, 
        "temperature": temperature, 
        "do_sample": False, 
    } 

    output = pipe(messages, **generation_args) 
    return output[0]['generated_text']