windows\svm-scale.exe -r train.txt.range predict.txt > predict_scale.txt
windows\svm-predict.exe -q predict_scale.txt train.txt.model output.txt